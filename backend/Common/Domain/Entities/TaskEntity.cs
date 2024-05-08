namespace Presentation.Common.Domain.Entities;
public class TaskEntity : BaseEntity
{
    public string Path { get; set; }
    public string Title { get; set; }

    public string ChallengeId { get; set; }
    public ChallengeEntity Challenge { get; set; }

    public ICollection<UserEntity> Users { get; set; }
}
