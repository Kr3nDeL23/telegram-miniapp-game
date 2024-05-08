namespace Presentation.Common.Domain.Configurations;
public class TelegramConfiguration
{
    public string Token { get; set; }
    public string WebUrl { get; set; }
    public string Domain { get; set; }
    public string Secret { get; set; }
    public long Owner { get; set; }
    public List<long> Admins { get; set; }

    public List<TelegramChannel> Channels { get; set; } = new();
}
public class TelegramChannel
{
    public long ChatId { get; set; }
    public string Name { get; set; }
    public string Link { get; set; }
}