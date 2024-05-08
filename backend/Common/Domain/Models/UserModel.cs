namespace Presentation.Common.Domain.Models;
public class UserModel : BaseModel
{
    public string Name { get; set; }
    public long TelegramId { get; set; }

    public long BalanceCoin { get; set; }

    public long AvailableEnergy { get; set; }
    public DateTime LastClickDate { get; set; }

    public string LimitEnergyLevelId { get; set; }
    public LevelModel LimitEnergyLevel { get; set; }

    public string MultipleClickLevelId { get; set; }
    public LevelModel MultipleClickLevel { get; set; }

    public string RechargeEnergyLevelId { get; set; }
    public LevelModel RechargeEnergyLevel { get; set; }
    public string RoBotLevelId { get; set; }
    public LevelModel RoBotLevel { get; set; }
    public string Image { get; set; }

    public string SquadId { get; set; }
    public SquadModel Squad { get; set; }

    public string LeagueId { get; set; }
    public LeagueModel League { get; set; }

    public string ParentId { get; set; }

    public string SkinId { get; set; }
    public SkinModel Skin { get; set; }

}
