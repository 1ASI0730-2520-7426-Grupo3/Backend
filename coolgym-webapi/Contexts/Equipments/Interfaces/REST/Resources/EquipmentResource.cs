namespace coolgym_webapi.Contexts.Equipments.Interfaces.REST.Resources;

/// <summary>
///     DTO: Representación completa de Equipment para respuestas de la API
///     Este es el objeto que se envía al cliente en formato JSON
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