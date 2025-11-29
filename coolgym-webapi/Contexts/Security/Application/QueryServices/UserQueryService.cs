using coolgym_webapi.Contexts.Security.Domain.Infrastructure;
using coolgym_webapi.Contexts.Security.Domain.Model;
using coolgym_webapi.Contexts.Security.Domain.Model.Entities;

namespace coolgym_webapi.Contexts.Security.Application.QueryServices;

/// <summary>
/// User query service - handles user queries
/// </summary>
public class UserQueryService(IUserRepository userRepository) : IUserQueryService
{
    public async Task<User?> GetByIdAsync(int userId)
    {
        var user = await userRepository.FindByIdAsync(userId);

        if (user == null || user.IsDeleted == SecurityDomainConstants.DeletedFlagTrue)
            return null;

        return user;
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        var user = await userRepository.FindByEmailAsync(email);

        if (user == null || user.IsDeleted == SecurityDomainConstants.DeletedFlagTrue)
            return null;

        return user;
    }
}
