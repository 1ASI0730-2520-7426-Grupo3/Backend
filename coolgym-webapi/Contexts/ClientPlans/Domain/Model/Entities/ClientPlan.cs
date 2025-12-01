using coolgym_webapi.Contexts.Shared.Domain.Model.Entities;

namespace coolgym_webapi.Contexts.ClientPlans.Domain.Model.Entities;

/// <summary>
/// ClientPlan entity - represents a subscription plan for clients
/// </summary>
public class ClientPlan : BaseEntity
{
    protected ClientPlan()
    {
        // EF Core constructor
        Name = string.Empty;
        Description = string.Empty;
    }

    public ClientPlan(
        string name,
        string description,
        decimal monthlyPrice,
        int maxEquipmentAccess,
        bool hasMaintenanceSupport,
        bool hasPrioritySupport)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Plan name cannot be empty");

        if (monthlyPrice < 0)
            throw new ArgumentException("Monthly price cannot be negative");

        if (maxEquipmentAccess < 0)
            throw new ArgumentException("Max equipment access cannot be negative");

        Name = name.Trim();
        Description = description?.Trim() ?? string.Empty;
        MonthlyPrice = monthlyPrice;
        MaxEquipmentAccess = maxEquipmentAccess;
        HasMaintenanceSupport = hasMaintenanceSupport;
        HasPrioritySupport = hasPrioritySupport;
        CreatedDate = DateTime.UtcNow;
    }

    public string Name { get; private set; }
    public string Description { get; private set; }
    public decimal MonthlyPrice { get; private set; }
    public int MaxEquipmentAccess { get; private set; }
    public bool HasMaintenanceSupport { get; private set; }
    public bool HasPrioritySupport { get; private set; }
}
