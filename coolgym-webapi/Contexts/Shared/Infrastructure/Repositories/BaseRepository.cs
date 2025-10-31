using coolgym_webapi.Contexts.Shared.Domain.Repositories;
using coolgym_webapi.Contexts.Shared.Infrastructure.Persistence.Configuration;
using Microsoft.EntityFrameworkCore;

namespace coolgym_webapi.Contexts.Shared.Infrastructure.Repositories;

public class BaseRepository<TEntity> : IBaseRepository<TEntity> where TEntity : class
{
    private readonly CoolgymContext _context;

    public BaseRepository(CoolgymContext context)
    {
        _context = context;
    }

    public async Task AddAsync(TEntity entity)
    {
        await _context.AddAsync(entity); //'insert into table value("'
    }

    public async Task<TEntity?> FindByIdAsync(int id)
    {
        // 'select * from table where id = ...'
        return await _context.Set<TEntity>().FindAsync(id);
    }

    public void Update(TEntity entity)
    {
        _context.Set<TEntity>().Update(entity); //'update table set ... where id = ...'
    }

    public void Remove(TEntity entity)
    {
        _context.Set<TEntity>().Remove(entity); //'delete from table where id = ...'
    }

    public async Task<IEnumerable<TEntity>> ListAsync()
    {
        // 'select * from table'
        return await _context.Set<TEntity>().ToListAsync();
    }
}