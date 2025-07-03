using ForumNotificationBot.PLL.Controllers;
using Telegram.Bot;
using Telegram.Bot.Types;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace ForumNotificationBot.PLL.Controllers
{
    public class CallbackQueryController
    {
        private readonly ITelegramBotClient _botClient;
        private readonly InlineKeyboardController _keyboardController;
        private readonly MessageController _messageController;

        private static ConcurrentDictionary<long, string> _userLanguages = new();

        public CallbackQueryController(ITelegramBotClient botClient, InlineKeyboardController keyboardController, MessageController messageController)
        {
            _botClient = botClient;
            _keyboardController = keyboardController;
            _messageController = messageController;
        }

        public async Task Handle(CallbackQuery query, CancellationToken ct)
        {
            var chatId = query.Message.Chat.Id;

            switch (query.Data)
            {
                case "lang_ru":
                    _userLanguages[chatId] = "ru";
                    await _botClient.EditMessageReplyMarkup(chatId, query.Message.MessageId, replyMarkup: null, cancellationToken: ct);
                    await _messageController.CheckUserRegistration(chatId, "ru", ct);
                    break;

                case "lang_en":
                    _userLanguages[chatId] = "en";
                    await _botClient.EditMessageReplyMarkup(chatId, query.Message.MessageId, replyMarkup: null, cancellationToken: ct);
                    await _messageController.CheckUserRegistration(chatId, "en", ct);
                    break;

                // Здесь можно убрать confirm_yes/confirm_no, так как логика теперь в CheckUserRegistration
                // Можно либо оставить для других целей, либо удалить

                default:
                    await _botClient.AnswerCallbackQuery(query.Id, "Неизвестная команда", cancellationToken: ct);
                    break;
            }
        }
    }
}
