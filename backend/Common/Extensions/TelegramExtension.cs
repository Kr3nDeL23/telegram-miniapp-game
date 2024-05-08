using System.Web;
using System.Text;
using System.Security.Cryptography;

using System.Text.Json;
using System.Text.Json.Serialization;
using HtmlAgilityPack;
using Presentation.Common.Constants;
using Microsoft.Extensions.Options;

namespace Presentation.Common.Extensions;
public static class TelegramExtension
{

    public static async Task<ChatInformationModel> GetChatInformation(string link)
    {
        try
        {
            HttpClient httpClient = new HttpClient();

            var response = await httpClient.SendAsync(
                new HttpRequestMessage(HttpMethod.Get, link));

            using var reader = new StreamReader(response.Content.ReadAsStream());

            var html = await reader.ReadToEndAsync();

            HtmlDocument document = new HtmlDocument();

            document.LoadHtml(html);

            HtmlNode channelName = document.DocumentNode
                .SelectSingleNode("//div[contains(@class, 'tgme_page_title')]/span");

            HtmlNode channelImage = document.DocumentNode
                .SelectSingleNode("//img[contains(@class, 'tgme_page_photo_image')]");

            if (channelImage != null && channelName != null)
            {
                string name = channelName.InnerText.Trim();
                string image = channelImage.Attributes["src"].Value;

                string extension = Path.GetExtension(image);

                if (!string.IsNullOrEmpty(extension))
                {
                    var path = Path.Combine(LocationConstant.LocationSquadImages
                        , FileExtension.GenerateRandomFileName(extension));

                    if (!Directory.Exists(LocationConstant.LocationSquadImages))
                        Directory.CreateDirectory(LocationConstant.LocationSquadImages);

                    await HttpExtension.DownloadWithUrl(image, path);

                    if (File.Exists(path))
                    {
                        return new ChatInformationModel
                        {
                            Title = name,
                            Image = path,
                        };
                    }

                }
                return new ChatInformationModel
                { Title = name, Image = LocationConstant.LocationDefaultImage };
            }
        }
        catch (Exception e) { Console.WriteLine(e); }

        return null;
    }
    public static bool ValidateWebApp(string query, string token, out WebAppUserRequest webAppUser)
    {
        try
        {
            var parameters = HttpUtility.ParseQueryString(query);

            var hash = parameters["hash"];
            var user = parameters["user"];
            var date = parameters["auth_date"];

            webAppUser = JsonSerializer.Deserialize<WebAppUserRequest>(user);

            DateTime authTime = DateExtension.UnixTimeStampToDateTime(
                Convert.ToDouble(date));

            if ((DateTime.UtcNow - authTime).TotalHours > 2)
                return false;

            parameters.Remove("hash");

            var sortedParams = parameters.AllKeys.OrderBy(x => x);
            var dataCheckString = string.Join("\n", sortedParams.Select(key => $"{key}={parameters[key]}"));

            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes("WebAppData"));
            var secret = hmac.ComputeHash(Encoding.UTF8.GetBytes(token));

            using var hmac2 = new HMACSHA256(secret);
            var calculatedHash = hmac2.ComputeHash(Encoding.UTF8.GetBytes(dataCheckString));

            return BitConverter.ToString(calculatedHash).Replace("-", "").ToLower() == hash;
        }
        catch (Exception)
        {
            webAppUser = new();
        }

        return false;
    }

}

public class WebAppUserRequest
{
    [JsonPropertyName("id")]
    public long ChatId { get; set; }

    [JsonPropertyName("first_name")]
    public string FirstName { get; set; }

    [JsonPropertyName("last_name")]
    public string LastName { get; set; }

    [JsonPropertyName("username")]
    public string Username { get; set; }

    [JsonPropertyName("language_code")]
    public string LanguageCode { get; set; }

    [JsonPropertyName("allows_write_to_pm")]
    public bool AllowsWriteToPm { get; set; }
}
public class ChatInformationModel
{
    public string Title { get; set; }
    public string Image { get; set; }
}