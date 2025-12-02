using coolgym_webapi.Contexts.Security.Domain.Infrastructure;
using coolgym_webapi.Contexts.Security.Domain.Model;
using coolgym_webapi.Contexts.Security.Domain.Model.Entities;
using coolgym_webapi.Contexts.Security.Domain.Model.ValueObjects;
using coolgym_webapi.Contexts.Shared.Infrastructure.Persistence.Configuration;
using Microsoft.EntityFrameworkCore;

namespace coolgym_webapi.Contexts.Security.Application.QueryServices;

/// <summary>
///     User query service - handles user queries
/// </summary>
public class UserQueryService(IUserRepository userRepository, CoolgymContext context) : IUserQueryService
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

    public async Task<(int currentUsage, int planLimit, string usageType)> GetUserUsageStatisticsAsync(int userId)
    {
        var user = await userRepository.FindByIdAsync(userId);

        if (user == null || user.IsDeleted == SecurityDomainConstants.DeletedFlagTrue)
            return (0, 0, "unknown");

        // Get the user's current plan
        var plan = user.ClientPlanId.HasValue
            ? await context.ClientPlans.FindAsync(user.ClientPlanId.Value)
            : null;

        if (plan == null)
            return (0, 0, "unknown");

        int currentUsage;
        string usageType;

        // For Clients: count active rental requests (machines rented)
        if (user.Role == UserRole.Client)
        {
            currentUsage = await context.RentalRequests
                .Where(r => r.ClientId == userId && r.IsDeleted == 0 && r.Status == "approved")
                .CountAsync();
            usageType = "machines";
        }
        // For Providers: count unique clients they're serving
        else if (user.Role == UserRole.Provider)
        {
            currentUsage = await context.RentalRequests
                .Where(r => r.ProviderId == userId && r.IsDeleted == 0 && r.Status == "approved")
                .Select(r => r.ClientId)
                .Distinct()
                .CountAsync();
            usageType = "clients";
        }
        else
        {
            return (0, 0, "unknown");
        }

        return (currentUsage, plan.MaxEquipmentAccess, usageType);
    }
}