using coolgym_webapi.Contexts.Shared.Domain.Repositories;
using coolgym_webapi.Contexts.Shared.Infrastructure.Persistence.Configuration;

namespace coolgym_webapi.Contexts.Shared.Infrastructure.Repositories;

public class UnitOfWork(CoolgymContext context) : IUnitOfWork
{
    public async Task CompleteAsync()
    {
        await context.SaveChangesAsync();
    }
}