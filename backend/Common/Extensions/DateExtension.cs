namespace Presentation.Common.Extensions;
public class DateExtension
{
    public static DateTime UnixTimeStampToDateTime(double unixTime)
    {
        return new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc)
            .AddSeconds(unixTime).ToLocalTime();
    }
}
