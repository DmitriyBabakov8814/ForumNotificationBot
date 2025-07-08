using ForumNotificationBot.DAL.Data;
using ForumNotificationBot.DAL.Repositories;
using ForumNotificationBot.PLL.Controllers;
using ForumNotificationBot.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using Telegram.Bot;

namespace ForumNotificationBot
{
    internal class Program
    {
        static async Task Main()
        {
            var host = Host.CreateDefaultBuilder()
                .ConfigureServices((_, services) =>
                {
                    // Telegram Bot
                    services.AddSingleton<ITelegramBotClient>(_ =>
                        new TelegramBotClient("7531691997:AAHOcOcZ0kKctKNF_Iwv8MHq0C0Bi8uNFeg"));

                    // EF Core + SQLite
                    services.AddDbContext<AppDbContext>(opt =>
                        opt.UseSqlite("Data Source=app.db"));
                    services.AddScoped<INotificationRepository, NotificationRepository>();

                    // RabbitMQ
                    var factory = new ConnectionFactory
                    {
                        HostName = "185.200.240.235",
                        Port = 5672,
                        UserName = "rabbit_user",
                        Password = "rabbitpass"
                    };
                    var connection = factory.CreateConnection();
                    var channel = connection.CreateModel();
                    channel.QueueDeclare(
                        queue: "notifications",
                        durable: true,
                        exclusive: false,
                        autoDelete: false,
                        arguments: null);
                    services.AddSingleton(channel);
                    services.AddHostedService<RabbitMqListener>();

                    // Telegram controllers & bot
                    services.AddSingleton<InlineKeyboardController>();
                    services.AddSingleton<MessageController>();
                    services.AddSingleton<CallbackQueryController>();
                    services.AddSingleton<VoiceMessageController>();
                    services.AddHostedService<Bot>();
                })
                .Build();

            // Создать БД и таблицу NotificationMessages
            using var scope = host.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            db.Database.EnsureCreated();

            await host.RunAsync();
        }
    }
}
