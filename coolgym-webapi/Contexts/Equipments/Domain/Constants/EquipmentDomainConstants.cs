namespace coolgym_webapi.Contexts.Equipments.Domain.Constants;

/// <summary>
/// Domain-wide constants for the Equipment bounded context.
/// Centralizes flags, status values and default configuration
/// to avoid magic numbers and magic strings.
/// </summary>
public static class EquipmentDomainConstants
{
    // Soft delete flags
    public const int DeletedFlagFalse = 0;
    public const int DeletedFlagTrue = 1;

    // Status values
    public const string StatusActive = "active";
    public const string StatusMaintenance = "maintenance";
    public const string StatusPendingMaintenance = "pending_maintenance";
    public const string StatusInactive = "inactive";

    // Default control configuration
    public const string DefaultControlPower = "off";
    public const string DefaultControlStatus = "Normal";
    public const int DefaultControlMinLevel = 1;
    public const int DefaultControlMaxLevel = 10;
    public const int DefaultControlInitialLevel = DefaultControlMinLevel;

    // Location constraints
    public const int MaxLocationNameLength = 100;
    public const int MaxLocationAddressLength = 200;
}