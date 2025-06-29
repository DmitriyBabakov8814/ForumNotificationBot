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
                    InlineKeyboardButton.WithCallbackData("Русский", "lang_ru"),
                    InlineKeyboardButton.WithCallbackData("English", "lang_en")
                }
            });
        }

        public InlineKeyboardMarkup GetConfirmationKeyboard(string language)
        {
            if (language == "en")
            {
                return new InlineKeyboardMarkup(new[]
                {
                    new[]
                    {
                        InlineKeyboardButton.WithCallbackData("Yes", "confirm_yes"),
                        InlineKeyboardButton.WithCallbackData("No", "confirm_no")
                    }
                });
            }
            else 
            {
                return new InlineKeyboardMarkup(new[]
                {
                    new[]
                    {
                        InlineKeyboardButton.WithCallbackData("Да", "confirm_yes"),
                        InlineKeyboardButton.WithCallbackData("Нет", "confirm_no")
                    }
                });
            }
        }
    }
}
