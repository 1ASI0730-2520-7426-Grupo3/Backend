namespace coolgym_webapi.Contexts.ClientPlans.Interfaces.REST.Resources;

/// <summary>
///     Resource (DTO) representing a client plan in API responses
/// </summary>
public record ClientPlanResource(
    int Id,
    string Name,
    string Description,
    decimal MonthlyPrice,
    int MaxEquipmentAccess,
    bool HasMaintenanceSupport,
    bool HasPrioritySupport
);