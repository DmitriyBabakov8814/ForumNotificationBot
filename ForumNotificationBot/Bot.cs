using ForumNotificationBot.PLL.Controllers;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
namespace ForumNotificationBot
{
    public class Bot: BackgroundService // Экземпляр TelegramBotClient, конфигурация
    {
        private ITelegramBotClient _botClient;
        private CallbackQueryController _queryController;
        private InlineKeyboardController _inlineKeyboardController;
        private MessageController _messageController;
        private VoiceMessageController _voiceController;

        public Bot(ITelegramBotClient botClient, CallbackQueryController queryController, InlineKeyboardController inlineKeyboardController, MessageController messageController, VoiceMessageController voiceController)
        {
            _botClient = botClient;
            _queryController = queryController;
            _inlineKeyboardController = inlineKeyboardController;
            _messageController = messageController;
            _voiceController = voiceController;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = { } 
            };

            _botClient.StartReceiving(HandleUpdateAsync, HandleErrorAsync, receiverOptions, cancellationToken: stoppingToken);
            Console.WriteLine("Бот запущен");
            return Task.CompletedTask;
        }

        public async Task HandleUpdateAsync(ITelegramBotClient botClient,  Update update, CancellationToken ct)
        {
            if (update.Type == UpdateType.Message)
            {
                await _messageController.Handle(update.Message, ct);
            }

            else if (update.Type == UpdateType.CallbackQuery)
            {
                await _queryController.Handle(update.CallbackQuery, ct);
            }
        }

        public Task HandleErrorAsync(ITelegramBotClient botClient, Exception ex, CancellationToken ct)
        {
            var error = ex switch
            {
                ApiRequestException apiEx => $"Telegram API Error:\n[{apiEx.ErrorCode}] {apiEx.Message}",
                _ => ex.ToString()
            };

            Console.WriteLine(error);
            Console.WriteLine("Ожидание 10 секунд перед повтором...");
            Thread.Sleep(1000);
            return Task.CompletedTask;
        }
    }
}
