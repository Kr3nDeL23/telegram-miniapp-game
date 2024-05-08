namespace Presentation.Common.Domain.Models;
public class SquadModel : BaseModel
{
    public string Name { get; set; }
    public string UserName { get; set; }

    public long BalanceCoin { get; set; }

    public string LeagueId { get; set; }
    public LeagueModel League { get; set; }
    
    public string Image { get; set; }

}
