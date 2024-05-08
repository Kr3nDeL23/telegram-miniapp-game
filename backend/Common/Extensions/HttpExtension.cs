namespace Presentation.Common.Extensions;
public class HttpExtension
{

    public static async Task DownloadWithUrl(string url, string dest)
    {
        using (var httpClient = new HttpClient())
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, new Uri(url)))
            {
                using (Stream contentStream = await (
                    await httpClient.SendAsync(request)).Content.ReadAsStreamAsync(),
                        stream = new FileStream(dest, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    await contentStream.CopyToAsync(stream);
                }
            }
        }
    }
}
