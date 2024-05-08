using System.Linq.Expressions;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

using Presentation.Common.Domain.Enums;
using Presentation.Common.Domain.Contexts;
using Presentation.Common.Domain.Entities;

namespace Presentation.Common.Repositories;
public class BaseRepository<T> where T : BaseEntity
{
    private ApplicationDBContext _context;

    public BaseRepository(ApplicationDBContext context)
    {
        _context = context;
    }


    public virtual async Task<T> AddAsync(T entity)
    {
        var create = await _context.Set<T>().AddAsync(entity);

        return create.Entity;
    }

    public IQueryable<T> Queryable(Expression<Func<T, bool>> predicate = null,
        Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null,
        Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
        int? skip = null, int? take = null)
    {
        IQueryable<T> query = GetQueryable(predicate, include);

        if (orderBy != null) query = orderBy(query);

        if (skip != null && skip.HasValue) query = query.Skip(skip.Value);

        if (take != null && take.HasValue) query = query.Take(take.Value);

        return query;
    }
    public virtual T Update(T entity)
    {
        _context.Set<T>().Update(entity);

        return entity;
    }
    public virtual async Task<T> GetAsync(
        Expression<Func<T, bool>> predicate = null,
        Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null)
    {
        IQueryable<T> query = GetQueryable(predicate, include);

        return await query.FirstOrDefaultAsync();
    }

    public async Task DeleteAsync(Expression<Func<T, bool>> predicate)
    {
        var entity = await GetAsync(predicate: predicate);

        entity.State = StateEnum.Deleted;
    }
    public async Task<int> CountAsync()
    {
        IQueryable<T> query = GetQueryable();

        return await query.CountAsync();
    }

    public async Task<int> CountAsync(Expression<Func<T, bool>> predicate)
    {
        IQueryable<T> query = GetQueryable(predicate);

        return await query.CountAsync(predicate);
    }
    public IQueryable<T> GetQueryable(Expression<Func<T, bool>> predicate = null, Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null)
    {
        IQueryable<T> query = _context.Set<T>();

        if (include != null) query = include(query);

        if (predicate != null) query = query.Where(predicate);

        return query;
    }
}
