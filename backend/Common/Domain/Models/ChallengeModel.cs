namespace Presentation.Common.Domain.Models;
public class ChallengeModel : BaseModel
{
    public string Name { get; set; }
    public string Image { get; set; }
    public long Bonus { get; set; }

    public bool IsCompleted { get; set; }
}
