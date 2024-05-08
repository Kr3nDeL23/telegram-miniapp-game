using Presentation.Common.Domain.Contexts;
using Presentation.Common.Domain.Entities;

namespace Presentation.Common.Repositories;
public class LeagueRepository : BaseRepository<LeagueEntity>
{
    public LeagueRepository(ApplicationDBContext context) : base(context)
    {
    }
}