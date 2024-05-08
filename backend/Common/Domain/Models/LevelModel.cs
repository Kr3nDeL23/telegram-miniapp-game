using Presentation.Common.Domain.Enums;

namespace Presentation.Common.Domain.Models;
public class LevelModel : BaseModel
{
    public int Level { get; set; }
    public int Value { get; set; }
    public long AvailableCoin { get; set; }

    public LevelTypeEnum LevelType { get; set; }
}
