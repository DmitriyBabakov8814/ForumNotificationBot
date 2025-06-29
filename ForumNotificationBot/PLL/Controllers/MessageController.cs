using ForumNotificationBot.PLL.Controllers;
using Telegram.Bot.Types;
using Telegram.Bot;

public class MessageController
{
    private readonly ITelegramBotClient _botClient;
    private readonly InlineKeyboardController _keyboardController;

    public MessageController(ITelegramBotClient botClient, InlineKeyboardController keyboardController)
    {
        _botClient = botClient;
        _keyboardController = keyboardController;
    }

    public async Task Handle(Message message, CancellationToken ct)
    {
        if (message.Text == "/start")
        {
            await _botClient.SendMessage(
                chatId: message.Chat.Id,
                text: "Выберите язык / Choose your Language:",
                replyMarkup: _keyboardController.GetLanguageSelectionKeyboard(),
                cancellationToken: ct);
        }
        else
        {
            await _botClient.SendMessage(
                chatId: message.Chat.Id,
                text: "Введите команду /start для начала работы / Enter the command /start to start working",
                cancellationToken: ct);
        }

        Console.WriteLine($"Сработал MessageController, текст сообщения: {message.Text}");
    }
}
