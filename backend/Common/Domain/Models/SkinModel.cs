
namespace Presentation.Common.Domain.Models;
public class SkinModel : BaseModel
{
    public string Title { get; set; }
    public long AvailableCoin { get; set; }
    public bool IsBought { get; set; }
    public string Image { get; set; }
}
