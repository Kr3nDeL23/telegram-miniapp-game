using Presentation.Common.Domain.Contexts;
using Presentation.Common.Domain.Entities;

namespace Presentation.Common.Repositories;
public class ChallengeRepository : BaseRepository<ChallengeEntity>
{
    public ChallengeRepository(ApplicationDBContext context) : base(context)
    {
    }
}
