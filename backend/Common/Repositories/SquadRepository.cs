using Presentation.Common.Domain.Contexts;
using Presentation.Common.Domain.Entities;

namespace Presentation.Common.Repositories;
public class SquadRepository : BaseRepository<SquadEntity>
{
    public SquadRepository(ApplicationDBContext context) : base(context)
    { }
}
