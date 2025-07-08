using ForumNotificationBot.DAL.Repositories;
using Telegram.Bot;
using Telegram.Bot.Types;
using System.Threading;
using System.Threading.Tasks;

namespace ForumNotificationBot.PLL.Controllers
{
    public class CallbackQueryController
    {
        private readonly ITelegramBotClient _botClient;
        private readonly InlineKeyboardController _keyboardController;
        private readonly INotificationRepository _notificationRepository;

        public CallbackQueryController(
            ITelegramBotClient botClient,
            InlineKeyboardController keyboardController,
            INotificationRepository notificationRepository)
        {
            _botClient = botClient;
            _keyboardController = keyboardController;
            _notificationRepository = notificationRepository;
        }

        public async Task Handle(CallbackQuery query, CancellationToken ct)
        {
            var chatId = query.Message.Chat.Id;
            string lang = query.Data == "lang_en" ? "en" : "ru";

            // Убираем клавиатуру
            await _botClient.EditMessageReplyMarkup(
                chatId: chatId,
                messageId: query.Message.MessageId,
                replyMarkup: null,
                cancellationToken: ct);

            bool subscribed = await _notificationRepository
                .ExistsByTelegramIdAsync(chatId.ToString());

            string text = subscribed switch
            {
                true when lang == "en" => "✅ You are subscribed to forum notifications.",
                true => "✅ Вы подписаны на уведомления форума.",
                false when lang == "en" => "🔒 You are not registered. Please visit https://example.com to register.",
                _ => "🔒 Вы не зарегистрированы. Перейдите на https://example.com для регистрации."
            };

            await _botClient.SendMessage(
                chatId: chatId,
                text: text,
                cancellationToken: ct);
        }

    }
}
