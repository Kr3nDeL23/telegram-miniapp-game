using Presentation.Common.Domain.Contexts;
using Presentation.Common.Domain.Entities;

namespace Presentation.Common.Repositories;
public class LevelRepository : BaseRepository<LevelEntity>
{
    public LevelRepository(ApplicationDBContext context) : base(context)
    {
    }
}