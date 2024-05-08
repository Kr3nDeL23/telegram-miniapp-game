namespace Presentation.Common.Domain.Entities;
public class ChallengeEntity : BaseEntity
{
    public string Name { get; set; }
    public string Image { get; set; }
    public long Bonus { get; set; }
    public ICollection<TaskEntity> Tasks { get; set; }
    public ICollection<UserEntity> Users { get; set; }
}
