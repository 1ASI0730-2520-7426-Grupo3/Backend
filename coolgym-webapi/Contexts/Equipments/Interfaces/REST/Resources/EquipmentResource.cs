namespace coolgym_webapi.Contexts.Equipments.Interfaces.REST.Resources;

/// <summary>
///     DTO: Complete Equipment representation for API responses.
///     This is the object sent to the client in JSON format.
/// </summary>
public record EquipmentResource(
    int Id,
    string Name,
    string Type,
    string Model,
    string Manufacturer,
    string Code,
    string SerialNumber,
    DateTime InstallationDate,
    int PowerWatts,
    string Status,
    bool IsPoweredOn,
    string ActiveStatus,
    string? Notes,
    string? Image,
    int? OwnerUserId,
    LocationResource Location,
    UsageStatsResource Usage,
    ControlSettingsResource Controls,
    MaintenanceInfoResource MaintenanceInfo,
    DateTime CreatedDate,
    DateTime? UpdatedDate
);

public record LocationResource(string Name, string Address);

public record UsageStatsResource(int TotalMinutes, int TodayMinutes, int CaloriesToday);

public record ControlSettingsResource(
    string Power,
    int CurrentLevel,
    int SetLevel,
    int MinLevelRange,
    int MaxLevelRange,
    string Status
);

public record MaintenanceInfoResource(DateTime? LastMaintenanceDate, DateTime? NextMaintenanceDate);