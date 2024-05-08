namespace Presentation.Common.Domain.Models;
public class TaskModel : BaseModel
{
    public string Path { get; set; }
    public string Title { get; set; }

    public string ChallengeId { get; set; }
    public bool IsCompleted { get; set; }

}
