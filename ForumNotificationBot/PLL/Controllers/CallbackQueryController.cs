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

        // Временно — заглушки для почты и UUID
        private readonly string _email = "example@mail.com";
        private readonly string _uuid = "123e4567-e89b-12d3-a456-426614174000";

        // Хранилище выбранного языка для пользователей
        private static ConcurrentDictionary<long, string> _userLanguages = new();

        public CallbackQueryController(ITelegramBotClient botClient, InlineKeyboardController keyboardController)
        {
            _botClient = botClient;
            _keyboardController = keyboardController;
        }

        public async Task Handle(CallbackQuery query, CancellationToken ct)
        {
            var chatId = query.Message.Chat.Id;

            switch (query.Data)
            {
                case "lang_ru":
                    _userLanguages[chatId] = "ru";
                    await _botClient.EditMessageReplyMarkup(chatId, query.Message.MessageId, replyMarkup: null, cancellationToken: ct);
                    await _botClient.SendMessage(
                        chatId,
                        $"Подтвердите, ваша ли это почта: {_email}\nUUID ключ: {_uuid}",
                        replyMarkup: _keyboardController.GetConfirmationKeyboard("ru"),
                        cancellationToken: ct);
                    break;

                case "lang_en":
                    _userLanguages[chatId] = "en";
                    await _botClient.EditMessageReplyMarkup(chatId, query.Message.MessageId, replyMarkup: null, cancellationToken: ct);
                    await _botClient.SendMessage(
                        chatId,
                        $"Please confirm, is this your email: {_email}\nUUID key: {_uuid}",
                        replyMarkup: _keyboardController.GetConfirmationKeyboard("en"),
                        cancellationToken: ct);
                    break;

                case "confirm_yes":
                    var langYes = _userLanguages.GetValueOrDefault(chatId, "ru");
                    var textYes = langYes == "en"
                        ? "You are now subscribed to forum notifications."
                        : "Вы подписаны на уведомления форума.";
                    await _botClient.SendMessage(chatId, textYes, cancellationToken: ct);
                    break;

                case "confirm_no":
                    var langNo = _userLanguages.GetValueOrDefault(chatId, "ru");
                    var textNo = langNo == "en"
                        ? "Please update your email on the forum: https://yourforum.example.com/edit-profile"
                        : "Пожалуйста, обновите свою почту на сайте: https://yourforum.example.com/edit-profile";
                    await _botClient.SendMessage(chatId, textNo, cancellationToken: ct);
                    break;
            }
        }
    }
}
