using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;

using System.Text;
using System.Text.RegularExpressions;

using Presentation.Common;
using Presentation.Common.Constants;
using Presentation.Common.Extensions;
using Presentation.Common.Domain.Entities;
using Presentation.Common.Domain.Configurations;

using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Polling;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;


namespace Presentation.Telegram;
public class UpdateHandler : IUpdateHandler
{
    private readonly UnitOfWork _unitOfWork;
    private readonly ITelegramBotClient _client;
    private readonly TelegramConfiguration _telegramConfiguration;

    private List<TelegramModel> Users = new();

    public UpdateHandler(ITelegramBotClient client, UnitOfWork unitOfWork, IOptions<TelegramConfiguration> telegramConfiguration)
    {
        _client = client;
        _unitOfWork = unitOfWork;
        _telegramConfiguration = telegramConfiguration.Value;
    }
    public async Task HandleUpdateAsync(ITelegramBotClient _, Update update, CancellationToken cancellationToken)
    {

        if (update.Type == UpdateType.Message || update.Type == UpdateType.CallbackQuery)
        {
            var RequestUser = update.Message != null ? update.Message.From : update.CallbackQuery.From;


            var requestUserModel = await createIfNotExistUser(RequestUser);

            if (requestUserModel.getValue<bool>("blocked") == true) return;

            if (requestUserModel.isRequestAllowed() == false)
            {
                if (requestUserModel.getValue<bool>("limited") == true) return;

                await sendMessage(
                    chatId: RequestUser.Id,
                    message: "❌ You cannot use the bot for a minute because of spam");

                requestUserModel.setValue("limited", true);
                return;
            }
            requestUserModel.setValue("limited", false);

            if (update.Type == UpdateType.Message && !string.IsNullOrEmpty(update.Message.Text)
                                        && update.Message.Text.ToLower().StartsWith("/start"))
            {
                var regex = new Regex(Regex.Escape("_"));
                var text = regex.Replace(update.Message.Text.Split().LastOrDefault(), "|", 1);

                var command = text.Split("|").FirstOrDefault();
                var parameter = text.Split("|").LastOrDefault();

                if (command.StartsWith("ref"))
                {
                    var parent = await _unitOfWork.UserRepository.GetAsync(x => x.Id == parameter);

                    if (parent != null && parent.TelegramId != RequestUser.Id)
                    {
                        var user = await _unitOfWork.UserRepository.GetAsync(x => x.TelegramId == RequestUser.Id);

                        if (user != null && user.Parent == null)
                        {
                            user.Parent = parent;
                            parent.BalanceCoin += 2500;

                            await _unitOfWork.SaveChangesAsync();
                        }
                    }
                }
            }
            if (await CheckUserJoinChannel(RequestUser) == false)
            {
                if (update.Type == UpdateType.Message) await sendMessage(
                    chatId: RequestUser.Id,
                    message: "❗ To continue working with the robot, first join the channels and try again",
                    replyMarkup: KeyboardConstant.KeyboardMarkupPanelJoin(_telegramConfiguration.Channels));

                else await _client.AnswerCallbackQueryAsync(
                    update.CallbackQuery.Id,
                    text: "❗ To continue working with the robot, first join the channels and try again"
                );

                return;
            }
        }
        if (update.Type == UpdateType.Message)
            await BotOnMessageReceived(update.Message, cancellationToken);
        else if (update.Type == UpdateType.CallbackQuery)
            await BotOnCallbackQueryReceived(update.CallbackQuery, cancellationToken);
    }
    private async Task BotOnMessageReceived(Message message, CancellationToken cancellationToken)
    {

        var RequestChat = message.Chat;
        var RequestUser = message.From;

        var userModel = await createIfNotExistUser(RequestUser);

        if (string.IsNullOrEmpty(message.Text) == false)
        {
            if (message.Text.ToLower().StartsWith("/start"))
            {
                userModel.setValue("step", string.Empty);

                var regex = new Regex(Regex.Escape("_"));
                var text = regex.Replace(message.Text.Split().LastOrDefault(), "|", 1);

                var command = text.Split("|").FirstOrDefault();
                var parameter = text.Split("|").LastOrDefault();

                if (command.StartsWith("join"))
                {
                    var squad = await _unitOfWork.SquadRepository.GetAsync(x => x.Id == parameter);

                    if (squad != null)
                    {
                        var user = await _unitOfWork.UserRepository.GetAsync(x => x.TelegramId == RequestUser.Id);

                        user.Squad = squad;

                        await _unitOfWork.SaveChangesAsync();

                        await sendMessage(
                            chatId: RequestUser.Id,
                            message: "You have successfully joined the squad ✅");
                    }
                    else await sendMessage(
                            chatId: RequestUser.Id,
                            message: "Can not found squad ⛔");
                }
                else if (command.StartsWith("createsquad"))
                {
                    await sendMessage(
                        chatId: RequestChat.Id,
                        message: "<b>🔗 Send me a link to a public chat or channel to join a squad </b>",
                        replyMarkup: KeyboardConstant.KeyboardMarkupPanelBack()
                    );

                    userModel.setValue("step", "join_squad");
                    return;
                }

                if (userModel.IsAdmin || userModel.IsOwner)
                {
                    await sendMessage(
                        chatId: RequestUser.Id,
                        message: "Hello Welcome To Admin Panel 👋",
                        replyMarkup: KeyboardConstant.KeyboardMarkupPanelAdmin()
                    );
                }
                await sendMessage(
                    chatId: RequestUser.Id,
                    message: "Hello Welcome To Robot 👋",
                    replyMarkup: KeyboardConstant.KeyboardMarkupPanelMain(
                        _telegramConfiguration.WebUrl)
                );
            }
            var step = (userModel.getValue<string>("step") ?? "").ToLower();

            if (step.StartsWith("join_squad"))
            {
                if (ValidationExtension.TelegramLinkValidation(message.Text))
                {
                    var username = message.Text.ToLower().Replace("https://t.me/", string.Empty).Split("/").FirstOrDefault();

                    var user = await _unitOfWork.UserRepository.GetAsync(x => x.TelegramId == RequestUser.Id);
                    var squad = await _unitOfWork.SquadRepository.GetAsync(x => x.UserName == username);

                    if (squad != null)
                    {
                        user.Squad = squad;
                    }
                    else
                    {
                        var chat = await TelegramExtension.GetChatInformation(message.Text);

                        if (chat == null)
                        {
                            await sendMessage(
                                chatId: RequestUser.Id,
                                message: @$"Chat not found. Please make sure it exists or try again later ❌",
                                replyMarkup: KeyboardConstant.KeyboardMarkupPanelBack()
                            );
                            return;
                        }
                        var league = await _unitOfWork.LeagueRepository.Queryable(
                            orderBy: o => o.OrderBy(x => x.AvailableCoin)).FirstOrDefaultAsync();

                        user.Squad = new SquadEntity
                        {
                            Name = chat.Title,
                            UserName = username,
                            Image = FileExtension
                                .ConvertFilePathForDatabase(chat.Image),
                            League = league
                        };
                        squad = user.Squad;
                    }
                    await _unitOfWork.SaveChangesAsync();

                    userModel.setValue("step", string.Empty);

                    await sendMessage(
                        chatId: RequestUser.Id,
                        message: $"<b>You Are Joined To <code>{squad.Name}</code> ✅</b>",
                        replyMarkup: KeyboardConstant.KeyboardMarkupPanelBack()
                    );

                }

                else await sendMessage(
                    chatId: RequestUser.Id,
                    message: $"Please enter a valid https link ⛔",
                    replyMarkup: KeyboardConstant.KeyboardMarkupPanelBack()
                );

            }
        }

        var userStep = (userModel.getValue<string>("step") ?? "").ToLower();

        if (userModel.IsAdmin || userModel.IsOwner)
        {
            if (userStep.StartsWith("public_message"))
            {
                var users = await _unitOfWork.UserRepository.Queryable().ToListAsync();
                await sendMessage(
                    chatId: RequestUser.Id,
                    message: "♻ started send message to all members ...",
                    replyMarkup: KeyboardConstant.KeyboardMarkupPanelBack()
                );
                new Thread(
                    async () =>
                    {
                        foreach (var user in users)
                        {
                            try
                            {
                                await _client.CopyMessageAsync(
                                    chatId: user.TelegramId,
                                    fromChatId: RequestUser.Id,
                                    messageId: message.MessageId
                                );
                            }
                            catch (Exception) { }
                            finally { await Task.Delay(1000); }
                        }
                        await sendMessage(
                            chatId: RequestUser.Id,
                            message: "✅ message sent to all users",
                            replyMarkup: KeyboardConstant.KeyboardMarkupPanelBack()
                        );
                    }
                ).Start();

                userModel.setValue("step", string.Empty);
            }
        }
    }
    private async Task BotOnCallbackQueryReceived(CallbackQuery callbackQuery, CancellationToken cancellationToken)
    {
        var RequestUser = callbackQuery.From;
        var RequestChat = callbackQuery.Message.Chat;

        var userModel = await createIfNotExistUser(RequestUser);

        if (callbackQuery.Data.StartsWith("PanelMain"))
        {
            userModel.setValue("step", string.Empty);

            await sendMessage(
                chatId: RequestUser.Id,
                message: "Hello Welcome To Robot 👋",
                replyMarkup: KeyboardConstant.KeyboardMarkupPanelMain(
                    _telegramConfiguration.WebUrl)
            );
        }
        if (userModel.IsAdmin || userModel.IsOwner)
        {
            if (callbackQuery.Data.StartsWith("AdminPublicMessage"))
            {
                await sendMessage(
                    chatId: RequestChat.Id,
                    message: "<b>🔗 Send me a public message to send all members </b>",
                    replyMarkup: KeyboardConstant.KeyboardMarkupPanelBack()
                );

                userModel.setValue("step", "public_message");
            }
            if (callbackQuery.Data.StartsWith("AdminGetUsers"))
            {
                var users = await _unitOfWork.UserRepository.Queryable().ToListAsync();

                var text = new StringBuilder();

                text.Append("[~] Count All users : " + users.Count() + "\n");

                foreach (var user in users) text.Append(user.Id)
                    .Append("\t").Append(user.TelegramId)
                    .Append("\t").Append(user.BalanceCoin).Append("\n");

                await System.IO.File.WriteAllTextAsync(LocationConstant.LocationUsersFile, text.ToString());

                await using Stream stream = System.IO.File.OpenRead(LocationConstant.LocationUsersFile);

                await SendDocument(
                    chatId: RequestChat.Id,
                    caption: @$"<b>
👪  All user informations in document

➖ Count All Users : {users.Count()}
                        </b>",
                    path: LocationConstant.LocationUsersFile);
            }
        }

        if (callbackQuery.Data.StartsWith("PanelSquad"))
        {
            var user = await _unitOfWork.UserRepository.GetAsync(x => x.TelegramId == RequestUser.Id);

            string squadMention = string.IsNullOrEmpty(user.SquadId) ? "<b>❗ You do not have a Squad .please Join a Squad </b>" : @$"<b>
<a href='T.me/{user.Squad.UserName}'>{user.Squad.Name}</a> Is Your Squad

➖ Mining Coins : {user.Squad.BalanceCoin}
➖ Count Members : {await _unitOfWork.UserRepository.Queryable(x => x.SquadId == user.SquadId).CountAsync()}

</b>";
            await sendMessage(
                chatId: RequestUser.Id,
                message: squadMention,
                replyMarkup: KeyboardConstant.KeyboardMarkupPanelSquad()
            );

        }

        if (callbackQuery.Data.StartsWith("JoinSquad"))
        {
            await sendMessage(
                chatId: RequestChat.Id,
                message: "<b>🔗 Send me a link to a public chat or channel to join a squad </b>",
                replyMarkup: KeyboardConstant.KeyboardMarkupPanelBack()
            );

            userModel.setValue("step", "join_squad");
        }
        if (callbackQuery.Data.StartsWith("PanelProfile"))
        {
            var user = await _unitOfWork.UserRepository.GetAsync(x => x.TelegramId == RequestUser.Id);
            var getMe = await _client.GetMeAsync();
            string message = @$"
🥋 User Profile 

➖ User ID : <code>{user.TelegramId}</code>
➖ Balance Coin : <code>{user.BalanceCoin}</code>
➖ League : <code>{user.League.Name}</code>

Invite Link :
🔗 <code>https://t.me/{getMe.Username}/?start=ref_{user.Id}</code>

❗ invite friends and get 2500 coin bonus
            ";
            await sendMessage(
                chatId: RequestUser.Id,
                message: message,
                replyMarkup: KeyboardConstant.KeyboardMarkupPanelBack()
            );

        }
    }
    public async Task SendDocument(long chatId, string path, string caption)
    {
        await using FileStream fileStream = new(path, FileMode.Open, FileAccess.Read, FileShare.Read);
        var name = path.Split(Path.DirectorySeparatorChar).Last();

        await _client.SendDocumentAsync(
            chatId: chatId,
            document: new InputFileStream(fileStream, name),
            caption: caption,
            parseMode: ParseMode.Html);
    }
    public async Task sendMessage(long chatId, string message, IReplyMarkup replyMarkup = null)
    {
        await _client.SendTextMessageAsync(
            chatId: chatId,
            text: $"<b>{message}</b>",
            replyMarkup: replyMarkup,
            cancellationToken: CancellationToken.None,
            parseMode: ParseMode.Html, disableWebPagePreview: true);
    }
    public async Task<bool> CheckUserJoinChannel(User user)
    {

        foreach (var channel in _telegramConfiguration.Channels)
        {
            try
            {
                var chatMember = await _client.GetChatMemberAsync(channel.ChatId, user.Id);
                if (chatMember.Status == ChatMemberStatus.Kicked
                    || chatMember.Status == ChatMemberStatus.Left
                    || chatMember.Status == ChatMemberStatus.Restricted) return false;
            }
            catch (ApiRequestException) { continue; }
        }

        return true;
    }
    public async Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        await Task.CompletedTask;
    }

    public async Task<TelegramModel> createIfNotExistUser(User user)
    {
        if (Users.Any(x => x.Id == user.Id) == false)
            Users.Add(
                new TelegramModel
                {
                    Id = user.Id,
                    IsOwner = _telegramConfiguration.Owner == user.Id,
                    IsAdmin = _telegramConfiguration.Admins.Contains(user.Id),
                    User = await createUserInDatabase(user)
                }
            );


        return Users.SingleOrDefault(x => x.Id == user.Id);
    }
    public async Task<UserEntity> createUserInDatabase(User user)
    {
        var getUser = await _unitOfWork.UserRepository.GetAsync(x => x.TelegramId == user.Id);

        if (getUser == null)
        {
            getUser = await _unitOfWork.UserRepository.AddAsync(
                new UserEntity
                { Name = user.FirstName, TelegramId = user.Id }
            );
            await _unitOfWork.SaveChangesAsync();
        }
        return getUser;
    }
}
public class TelegramModel
{
    public long Id { get; set; }
    public bool IsOwner { get; set; }
    public bool IsAdmin { get; set; }
    public UserEntity User { get; set; }
    public List<DateTime> RequestHistory { get; set; } = new();
    public Dictionary<string, object> Values { get; set; } = new();

    public T getValue<T>(string key)
    {
        if (Values.Keys.Contains(key) == true)
            if (Values.TryGetValue(key, out object result)) return (T)result;

        return default;
    }
    public bool isRequestAllowed()
    {
        RequestHistory.Add(DateTime.UtcNow);

        var requestsInTimeWindow = RequestHistory.FindAll(time => time >= DateTime.UtcNow - TimeSpan.FromMinutes(1));

        if (requestsInTimeWindow.Count > 20) return false;

        return true;

    }
    public void setValue(string key, object value)
    {
        if (Values.Keys.Contains(key) == false)
            Values.Add(key, value);

        else Values[key] = value;
    }
}
