using Telegram.Bot;
using Telegram.Bot.Types;
using System.Threading;
using System.Threading.Tasks;
using ForumNotificationBot.DAL.Repositories;

namespace ForumNotificationBot.PLL.Controllers
{
    public class MessageController
    {
        private readonly ITelegramBotClient _botClient;
        private readonly InlineKeyboardController _keyboardController;
        private readonly IUserRepository _userRepository;

        public MessageController(ITelegramBotClient botClient, InlineKeyboardController keyboardController, IUserRepository userRepository)
        {
            _botClient = botClient;
            _keyboardController = keyboardController;
            _userRepository = userRepository;
        }

        public async Task Handle(Message message, CancellationToken ct)
        {
            if (message.Text == "/start")
            {
                // Отправляем выбор языка
                await _botClient.SendMessage(
                    message.Chat.Id,
                    "Выберите язык / Choose your Language:",
                    replyMarkup: _keyboardController.GetLanguageSelectionKeyboard(),
                    cancellationToken: ct
                );
            }
            else
            {
                await _botClient.SendMessage(
                    message.Chat.Id,
                    "Введите команду /start для начала работы / Enter the command /start to start working",
                    cancellationToken: ct);
            }
        }

        // Метод для вызова после выбора языка из CallbackQuery, с проверкой пользователя
        public async Task CheckUserRegistration(long chatId, string language, CancellationToken ct)
        {
            bool userExists = await _userRepository.ExistsByTelegramIdAsync(chatId);

            if (userExists)
            {
                var text = language == "en"
                    ? "Your account is successfully subscribed to forum notifications."
                    : "Ваш аккаунт успешно подписан на уведомления форума.";

                await _botClient.SendMessage(chatId, text, cancellationToken: ct);
            }
            else
            {
                var text = language == "en"
                    ? "Unfortunately, you are not registered on the forum. Please register here: https://yourforum.example.com and then press /start again."
                    : "К сожалению, вы не зарегистрированы на форуме. Пожалуйста, зарегистрируйтесь здесь: https://yourforum.example.com и снова нажмите /start.";

                await _botClient.SendMessage(chatId, text, cancellationToken: ct);
            }
        }
    }
}
