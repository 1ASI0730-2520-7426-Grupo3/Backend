using coolgym_webapi.Contexts.Shared.Domain.Repositories;
using coolgym_webapi.Contexts.Shared.Infrastructure.Persistence.Configuration;
using Microsoft.EntityFrameworkCore;

namespace coolgym_webapi.Contexts.Shared.Infrastructure.Repositories;

public class BaseRepository<TEntity>(CoolgymContext context) 
    : IBaseRepository<TEntity> where TEntity : class
{

    public async Task AddAsync(TEntity entity)
    {
        await context.AddAsync(entity); //'insert into table value'
    }

    public async Task<TEntity?> FindByIdAsync(int id)
    {
        // 'select * from table where id = ...'
        return await context.Set<TEntity>().FindAsync(id);
    }

    public void Update(TEntity entity)
    {
        context.Set<TEntity>().Update(entity); //'update table set ... where id = ...'
    }

    public void Remove(TEntity entity)
    {
        context.Set<TEntity>().Remove(entity); //'delete from table where id = ...'
    }

    public async Task<IEnumerable<TEntity>> ListAsync()
    {
        // 'select * from table'
        return await context.Set<TEntity>().ToListAsync();
    }
}