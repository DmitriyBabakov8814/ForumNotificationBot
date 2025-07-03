using ForumNotificationBot.DAL.Data;
using ForumNotificationBot.DAL.Repositories;
using ForumNotificationBot.PLL.Controllers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using ForumNotificationBot.BLL;
using Telegram.Bot;
using ForumNotificationBot.Services;

// Убрал using Microsoft.EntityFrameworkCore.Metadata; так как он вызывает конфликт и не нужен здесь

namespace ForumNotificationBot
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var host = new HostBuilder()
                .ConfigureServices((context, services) =>
                {
                    // --- Telegram Bot Client ---
                    var token = "7531691997:AAHOcOcZ0kKctKNF_Iwv8MHq0C0Bi8uNFeg";
                    services.AddSingleton<ITelegramBotClient>(_ => new TelegramBotClient(token));

                    // --- EF Core / SQLite ---
                    services.AddDbContext<AppDbContext>(opt =>
                        opt.UseSqlite("Data Source=app.db"));
                    services.AddScoped<IUserRepository, UserRepository>();

                    // --- HTTP Client for PATCH requests ---
                    services.AddHttpClient();

                    // --- RabbitMQ setup ---
                    var factory = new ConnectionFactory
                    {
                        HostName = "localhost"
                        // При необходимости: UserName, Password, VirtualHost и т.п.
                    };
                    IConnection connection = factory.CreateConnection();

                    // Явно указываем RabbitMQ.Client.IModel, чтобы убрать неоднозначность
                    RabbitMQ.Client.IModel channel = connection.CreateModel();
                    channel.QueueDeclare(
                        queue: "notifications_queue",
                        durable: true,
                        exclusive: false,
                        autoDelete: false,
                        arguments: null
                    );
                    services.AddSingleton(channel);
                    services.AddScoped<RabbitMqListener>();

                    // --- Telegram Controllers ---
                    services.AddSingleton<InlineKeyboardController>();
                    services.AddScoped<MessageController>();
                    services.AddScoped<CallbackQueryController>();
                    services.AddScoped<VoiceMessageController>();

                    // --- Hosted Service: запускаем Bot как фоновый сервис ---
                    services.AddHostedService<Bot>();
                })
                .UseConsoleLifetime()
                .Build();

            Console.WriteLine("Сервис запущен");
            await host.RunAsync();
            Console.WriteLine("Сервис остановлен");
        }
    }
}
