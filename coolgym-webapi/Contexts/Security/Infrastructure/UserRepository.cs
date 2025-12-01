using coolgym_webapi.Contexts.Security.Domain.Infrastructure;
using coolgym_webapi.Contexts.Security.Domain.Model;
using coolgym_webapi.Contexts.Security.Domain.Model.Entities;
using coolgym_webapi.Contexts.Shared.Infrastructure.Persistence.Configuration;
using coolgym_webapi.Contexts.Shared.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace coolgym_webapi.Contexts.Security.Infrastructure;

public class UserRepository(CoolgymContext context)
    : BaseRepository<User>(context), IUserRepository
{
    public async Task<User?> FindByEmailAsync(string email)
    {
        return await context.Users
            .Where(u => u.IsDeleted == SecurityDomainConstants.DeletedFlagFalse)
            .FirstOrDefaultAsync(u => u.Email.Value == email.ToLowerInvariant());
    }

    public async Task<User?> FindByUsernameAsync(string username)
    {
        return await context.Users
            .Where(u => u.IsDeleted == SecurityDomainConstants.DeletedFlagFalse)
            .FirstOrDefaultAsync(u => u.Username == username);
    }

    public async Task<bool> ExistsByEmailAsync(string email)
    {
        return await context.Users
            .Where(u => u.IsDeleted == SecurityDomainConstants.DeletedFlagFalse)
            .AnyAsync(u => u.Email.Value == email.ToLowerInvariant());
    }

    public async Task<bool> ExistsByUsernameAsync(string username)
    {
        return await context.Users
            .Where(u => u.IsDeleted == SecurityDomainConstants.DeletedFlagFalse)
            .AnyAsync(u => u.Username == username);
    }
}