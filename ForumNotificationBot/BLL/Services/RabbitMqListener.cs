using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using ForumNotificationBot.BLL.Models;
using ForumNotificationBot.DAL.Repositories;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using ForumNotificationBot.DAL.Data;

namespace ForumNotificationBot.Services
{
    public class RabbitMqListener
    {
        private readonly RabbitMQ.Client.IModel _channel;
        private readonly IUserRepository _userRepository;
        private readonly HttpClient _httpClient;

        public RabbitMqListener(RabbitMQ.Client.IModel channel, IUserRepository userRepository, HttpClient httpClient)
        {
            _channel = channel;
            _userRepository = userRepository;
            _httpClient = httpClient;
        }

        public void StartListening()
        {
            var consumer = new EventingBasicConsumer(_channel);

            consumer.Received += async (sender, ea) =>
            {
                var body = ea.Body.ToArray();
                var json = Encoding.UTF8.GetString(body);

                NotificationMessage notification;
                try
                {
                    notification = JsonSerializer.Deserialize<NotificationMessage>(json);
                }
                catch
                {
                    _channel.BasicAck(ea.DeliveryTag, false);
                    return;
                }

                if (notification == null)
                {
                    _channel.BasicAck(ea.DeliveryTag, false);
                    return;
                }

                bool userExists = false;

                if (notification.RecipientTelegramId != null)
                {
                    userExists = await _userRepository.ExistsByTelegramIdAsync(notification.RecipientTelegramId.Value);
                    if (!userExists)
                    {
                        await _userRepository.AddUserAsync(new DAL.Data.User
                        {
                            TelegramId = notification.RecipientTelegramId,
                            Email = notification.RecipientEmail,
                            UserId = notification.UserId
                        });
                    }
                }
                else if (!string.IsNullOrEmpty(notification.RecipientEmail))
                {
                    userExists = await _userRepository.ExistsByEmailAsync(notification.RecipientEmail);
                    if (!userExists)
                    {
                        await _userRepository.AddUserAsync(new DAL.Data.User
                        {
                            Email = notification.RecipientEmail,
                            UserId = notification.UserId
                        });
                    }
                }

                // PATCH-запрос к форуму
                var patchPayload = JsonSerializer.Serialize(new { status = "sent" });
                var content = new StringContent(patchPayload, Encoding.UTF8, "application/json");
                var url = $"https://yourforum.example.com/api/notifications/{notification.Id}/update-status/";

                try
                {
                    await _httpClient.PatchAsync(url, content);
                }
                catch
                {
                    // логика при ошибке
                }

                _channel.BasicAck(ea.DeliveryTag, false);
            };

            _channel.BasicConsume(queue: "notifications_queue", autoAck: false, consumer: consumer);
        }
    }
}
