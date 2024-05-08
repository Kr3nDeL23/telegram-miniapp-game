namespace Presentation.Common.Extensions;
public class ValidationExtension
{
    public static bool TelegramLinkValidation(string link)
    {
        if (Uri.TryCreate(link, UriKind.Absolute, out Uri uriResult) &&
            uriResult.Scheme == Uri.UriSchemeHttps && uriResult.Host.ToLower() == "t.me") return true;

        return false;
    }
}
