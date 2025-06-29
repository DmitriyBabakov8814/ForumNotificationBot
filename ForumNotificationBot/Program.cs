using ForumNotificationBot.PLL.Controllers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Telegram.Bot;

namespace ForumNotificationBot
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var host = new HostBuilder()
                .ConfigureServices(services =>
                {
                    var token = "7531691997:AAHOcOcZ0kKctKNF_Iwv8MHq0C0Bi8uNFeg";

                    services.AddSingleton<ITelegramBotClient>(new TelegramBotClient(token));

                    services.AddSingleton<InlineKeyboardController>();
                    services.AddSingleton<MessageController>();
                    services.AddSingleton<CallbackQueryController>();
                    services.AddSingleton<VoiceMessageController>(); 

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
