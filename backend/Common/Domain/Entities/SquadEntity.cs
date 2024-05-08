
namespace Presentation.Common.Domain.Entities;
public class SquadEntity : BaseEntity
{
    public string Name { get; set; }
    public string UserName { get; set; }

    public long BalanceCoin { get; set; }

    public string LeagueId { get; set; }
    public LeagueEntity League { get; set; }
    public string Image { get; set; }

    public List<UserEntity> Members { get; set; } = new();

}
