using Presentation.Common.Domain.Contexts;
using Presentation.Common.Domain.Entities;

namespace Presentation.Common.Repositories;
public class TaskRepository : BaseRepository<TaskEntity>
{
    public TaskRepository(ApplicationDBContext context) : base(context)
    {
    }
}
