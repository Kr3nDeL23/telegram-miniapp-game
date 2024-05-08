using Presentation.Common.Domain.Configurations;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace Presentation.Common.Constants;
public class KeyboardConstant
{
    public static InlineKeyboardMarkup KeyboardMarkupPanelJoin(List<TelegramChannel> channels)
    {
        var keyboard = new List<List<InlineKeyboardButton>>();

        foreach (var channel in channels)
        {
            keyboard.Add(new List<InlineKeyboardButton>(){
                InlineKeyboardButton.WithUrl(channel.Name, channel.Link)
            });
        }

        keyboard.Add(new List<InlineKeyboardButton>(){
            InlineKeyboardButton.WithCallbackData("Im Joined 🖐" ,"PanelMain")
        });


        return new InlineKeyboardMarkup(keyboard);
    }
    public static InlineKeyboardMarkup KeyboardMarkupPanelMain(string url)
    {
        return new(
            new[]
            {
                new []
                {
                    InlineKeyboardButton.WithWebApp("Start Game 🎮",new WebAppInfo{Url = url}),
                },
                new []
                {
                    InlineKeyboardButton.WithCallbackData("Squad 🤼‍♀️", "PanelSquad"),
                    InlineKeyboardButton.WithCallbackData("Profile 🥋", "PanelProfile"),
                },

            }
        );
    }
    public static InlineKeyboardMarkup KeyboardMarkupPanelAdmin()
    {
        return new(
            new[]
            {
                new []
                {
                    InlineKeyboardButton.WithCallbackData("Get Users 🖐", "AdminGetUsers"),
                },
                new []
                {
                    InlineKeyboardButton.WithCallbackData("Public Message 🌐", "AdminPublicMessage"),
                },
            }
        );
    }
    public static InlineKeyboardMarkup KeyboardMarkupPanelSquad()
    {
        return new(
            new[]
            {
                new []
                {
                    InlineKeyboardButton.WithCallbackData("Join&Create Squad 🤼‍♀️","JoinSquad"),
                },
                new []
                {
                    InlineKeyboardButton.WithCallbackData("🔙","PanelMain"),
                },
            }
        );
    }
    public static InlineKeyboardMarkup KeyboardMarkupPanelBack(string callback = "PanelMain")
    {
        return new(
            new[]
            {
                new []
                {
                    InlineKeyboardButton.WithCallbackData("🔙",callback),
                },
            }
        );
    }


}
