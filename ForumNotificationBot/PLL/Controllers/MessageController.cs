using Telegram.Bot;
using Telegram.Bot.Types;
using System.Threading;
using System.Threading.Tasks;

namespace ForumNotificationBot.PLL.Controllers
{
    public class MessageController
    {
        private readonly ITelegramBotClient _botClient;
        private readonly InlineKeyboardController _keyboardController;

        public MessageController(
            ITelegramBotClient botClient,
            InlineKeyboardController keyboardController)
        {
            _botClient = botClient;
            _keyboardController = keyboardController;
        }

        public async Task Handle(Message message, CancellationToken ct)
        {
            if (message.Text == "/start")
            {
                await _botClient.SendMessage(
                    message.Chat.Id,
                    "Выберите язык / Choose your Language:",
                    replyMarkup: _keyboardController.GetLanguageSelectionKeyboard(),
                    cancellationToken: ct);
            }
            else
            {
                await _botClient.SendMessage(
                    message.Chat.Id,
                    "Введите команду /start для начала работы / Enter /start to begin.",
                    cancellationToken: ct);
            }
        }
    }
}
