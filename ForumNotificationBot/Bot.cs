using ForumNotificationBot.PLL.Controllers;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace ForumNotificationBot
{
    public class Bot : BackgroundService
    {
        private readonly ITelegramBotClient _botClient;
        private readonly MessageController _messageController;
        private readonly CallbackQueryController _callbackController;

        public Bot(
            ITelegramBotClient botClient,
            MessageController messageController,
            CallbackQueryController callbackController)
        {
            _botClient = botClient;
            _messageController = messageController;
            _callbackController = callbackController;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var receiverOptions = new ReceiverOptions { AllowedUpdates = { } };
            _botClient.StartReceiving(
                HandleUpdateAsync,
                HandleErrorAsync,
                receiverOptions,
                cancellationToken: stoppingToken);
            Console.WriteLine("🤖 Telegram-бот запущен.");
            return Task.CompletedTask;
        }

        private async Task HandleUpdateAsync(
            ITelegramBotClient botClient,
            Update update,
            CancellationToken ct)
        {
            if (update.Type == UpdateType.Message && update.Message.Text != null)
                await _messageController.Handle(update.Message, ct);
            else if (update.Type == UpdateType.CallbackQuery)
                await _callbackController.Handle(update.CallbackQuery, ct);
        }

        private Task HandleErrorAsync(ITelegramBotClient botClient, Exception ex, CancellationToken ct)
        {
            var errorMessage = ex switch
            {
                ApiRequestException apiEx => $"Telegram API Error:\n[{apiEx.ErrorCode}] {apiEx.Message}",
                _ => ex.ToString()
            };
            Console.WriteLine(errorMessage);
            return Task.CompletedTask;
        }
    }
}
