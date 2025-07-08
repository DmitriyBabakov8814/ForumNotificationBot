using ForumNotificationBot.BLL.Models;
using ForumNotificationBot.DAL.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Text.Json;
using System.Threading;
using Telegram.Bot;

namespace ForumNotificationBot.Services
{
    public class RabbitMqListener : BackgroundService
    {
        private readonly IModel _channel;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ITelegramBotClient _botClient;
        private const string QueueName = "notifications";

        public RabbitMqListener(
            IModel channel,
            IServiceScopeFactory scopeFactory,
            ITelegramBotClient botClient)
        {
            _channel = channel ?? throw new ArgumentNullException(nameof(channel));
            _scopeFactory = scopeFactory ?? throw new ArgumentNullException(nameof(scopeFactory));
            _botClient = botClient ?? throw new ArgumentNullException(nameof(botClient));

            Console.WriteLine(_channel.IsOpen
                ? "✅ Подключение к RabbitMQ установлено."
                : "❌ Нет подключения к RabbitMQ.");
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (!_channel.IsOpen)
            {
                Console.WriteLine("❌ Канал RabbitMQ закрыт. Слушатель не запущен.");
                return Task.CompletedTask;
            }

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += OnMessageReceived;
            _channel.BasicConsume(queue: QueueName, autoAck: false, consumer: consumer);

            Console.WriteLine("👂 Слушатель RabbitMQ запущен.");
            return Task.CompletedTask;
        }

        private async void OnMessageReceived(object sender, BasicDeliverEventArgs ea)
        {
            var json = Encoding.UTF8.GetString(ea.Body.ToArray());
            Console.WriteLine("📩 Получено сообщение из RabbitMQ:");
            Console.WriteLine(json);

            try
            {
                var notification = JsonSerializer.Deserialize<NotificationEntity>(json);
                if (notification == null)
                    throw new Exception("Десериализация вернула null.");

                using var scope = _scopeFactory.CreateScope();
                var repo = scope.ServiceProvider.GetRequiredService<INotificationRepository>();

                await repo.AddAsync(notification);
                Console.WriteLine($"✅ Уведомление для {notification.RecipientTelegramId} сохранено в БД.");

                bool subscribed = await repo.ExistsByTelegramIdAsync(notification.RecipientTelegramId);
                if (subscribed && long.TryParse(notification.RecipientTelegramId, out var chatId))
                {
                    await _botClient.SendMessage(
                        chatId,
                        $"📬 Новое уведомление:\n{notification.Title}\n{notification.Message}");
                    Console.WriteLine($"📤 Уведено Telegram ID: {chatId}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ Ошибка при обработке уведомления: " + ex.Message);
            }
            finally
            {
                _channel.BasicAck(ea.DeliveryTag, false);
            }
        }
    }
}
