using Microsoft.EntityFrameworkCore;
using Presentation.Common.Domain.Enums;
using Presentation.Common.Domain.Contexts;
using Presentation.Common.Domain.Entities;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;
using Presentation.Common.Constants;
using Presentation.Common.Extensions;

namespace Presentation.Common.Repositories;
public class UserRepository : BaseRepository<UserEntity>
{
    private readonly ApplicationDBContext _context;
    public UserRepository(ApplicationDBContext context) : base(context)
    {
        _context = context;
    }
    public override async Task<UserEntity> AddAsync(UserEntity entity)
    {
        var user = await base.AddAsync(entity);

        var levels = await _context.Levels
            .OrderBy(x => x.Level).ToListAsync();

        entity.LimitEnergyLevel = levels.FirstOrDefault(
            x => x.LevelType == LevelTypeEnum.LimitEnergyLevel);

        entity.MultipleClickLevel = levels.FirstOrDefault(
            x => x.LevelType == LevelTypeEnum.MultipleClickLevel);

        entity.RechargeEnergyLevel = levels.FirstOrDefault(
            x => x.LevelType == LevelTypeEnum.RechargeEnergyLevel);

        if (entity.LimitEnergyLevel != null)
            entity.AvailableEnergy = entity.LimitEnergyLevel.Value;

        if (entity.League == null)
            entity.League = await _context.Leagues
                .OrderBy(x => x.AvailableCoin).FirstOrDefaultAsync();

        if (entity.Skin == null)
        {
            entity.Skin = await _context.Skins
                .OrderBy(x => x.AvailableCoin).OrderBy(x => x.AvailableCoin).FirstOrDefaultAsync();

            entity.Skins.Add(entity.Skin);
        }

        if (string.IsNullOrEmpty(entity.Image))
            entity.Image = FileExtension.ConvertFilePathForDatabase(LocationConstant.LocationDefaultImage);

        return user;
    }
    public override async Task<UserEntity> GetAsync(Expression<Func<UserEntity, bool>> predicate = null, Func<IQueryable<UserEntity>, IIncludableQueryable<UserEntity, object>> include = null)
    {
        var user = await base.GetAsync(predicate, include);

        if (user != null)
        {
            var difference = Math.Max((DateTime.UtcNow - user.LastClickDate).TotalSeconds, 0);

            user.AvailableEnergy = Math.Min(user.AvailableEnergy +
                (Convert.ToInt64(difference) * user.RechargeEnergyLevel.Value), user.LimitEnergyLevel.Value);
        }
        return user;
    }
}
