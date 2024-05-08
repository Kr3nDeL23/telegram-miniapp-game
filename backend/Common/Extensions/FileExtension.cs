using Presentation.Common.Constants;

namespace Presentation.Common.Extensions;
public class FileExtension
{
    public static string GenerateRandomFileName(string extension)
    {
        return DateTime.UtcNow.ToFileTime().ToString() + extension;
    }
    public static string ConvertFilePathForDatabase(string path)
    {
        Console.WriteLine(path);
        Console.WriteLine(LocationConstant.LocationRoot);

        return path.Replace(LocationConstant.LocationRoot, string.Empty);
    }
}
