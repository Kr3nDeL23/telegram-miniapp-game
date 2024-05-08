using Presentation.Common.Domain.Contexts;
using Presentation.Common.Domain.Entities;

namespace Presentation.Common.Repositories;
public class SkinRepository : BaseRepository<SkinEntity>
{
    public SkinRepository(ApplicationDBContext context) : base(context)
    { }
}
