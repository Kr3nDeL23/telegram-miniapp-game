namespace Presentation.Common.Constants;
public class LocationConstant
{
    public static string LocationRoot = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
    public static string LocationSquadImages = Path.Combine(LocationRoot, "Images", "squads");
    public static string LocationUsersFile = Path.Combine(Directory.GetCurrentDirectory() ,"users.txt");
    public static string LocationDefaultImage = Path.Combine(LocationRoot, "Images", "default_image.jpg");
}
