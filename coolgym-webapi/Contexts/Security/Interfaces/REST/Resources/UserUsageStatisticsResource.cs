namespace coolgym_webapi.Contexts.Security.Interfaces.REST.Resources;

/// <summary>
///     Resource representing user's current usage statistics against their plan limits
/// </summary>
public record UserUsageStatisticsResource(
    int CurrentUsage,
    int PlanLimit,
    string UsageType
);
