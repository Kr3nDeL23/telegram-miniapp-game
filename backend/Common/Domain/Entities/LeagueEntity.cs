namespace Presentation.Common.Domain.Entities;
public class LeagueEntity : BaseEntity
{
    public string Name { get; set; }
    public long AvailableCoin { get; set; }

    public string Image { get; set; }
}
