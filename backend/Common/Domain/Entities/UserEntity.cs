namespace Presentation.Common.Domain.Entities;
public class UserEntity : BaseEntity
{
    public string Name { get; set; }
    public long TelegramId { get; set; }

    public long BalanceCoin { get; set; }

    public long AvailableEnergy { get; set; }
    public DateTime LastClickDate { get; set; }

    public string LimitEnergyLevelId { get; set; }
    public LevelEntity LimitEnergyLevel { get; set; }

    public string MultipleClickLevelId { get; set; }
    public LevelEntity MultipleClickLevel { get; set; }

    public string RechargeEnergyLevelId { get; set; }
    public LevelEntity RechargeEnergyLevel { get; set; }

    public string RoBotLevelId { get; set; }
    public LevelEntity RoBotLevel { get; set; }

    public string Image { get; set; }

    public string SquadId { get; set; }
    public SquadEntity Squad { get; set; }

    public string LeagueId { get; set; }
    public LeagueEntity League { get; set; }

    public string ParentId { get; set; }
    public UserEntity Parent { get; set; }

    public string SkinId { get; set; }
    public SkinEntity Skin { get; set; }

    public List<SkinEntity> Skins { get; set; } = new();
    public List<UserEntity> Children { get; set; } = new();

    public ICollection<TaskEntity> Tasks { get; set; }
    public ICollection<ChallengeEntity> Challenges { get; set; }
}
