using Telegram.Bot.Types.ReplyMarkups;

namespace ForumNotificationBot.PLL.Controllers
{
    public class InlineKeyboardController
    {
        public InlineKeyboardMarkup GetLanguageSelectionKeyboard()
        {
            return new InlineKeyboardMarkup(new[]
            {
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("Русский 🇷🇺", "lang_ru"),
                    InlineKeyboardButton.WithCallbackData("English 🇬🇧", "lang_en")
                }
            });
        }
    }
}
