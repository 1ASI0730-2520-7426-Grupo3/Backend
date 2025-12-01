using coolgym_webapi.Contexts.Security.Domain.Model.Entities;

namespace coolgym_webapi.Contexts.Security.Application.QueryServices;

/// <summary>
/// User query service interface - handles user queries
/// </summary>
public interface IUserQueryService
{
    Task<User?> GetByIdAsync(int userId);
    Task<User?> GetByEmailAsync(string email);
}
