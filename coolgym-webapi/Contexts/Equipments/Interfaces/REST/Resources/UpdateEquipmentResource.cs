namespace coolgym_webapi.Contexts.Equipments.Interfaces.REST.Resources;

/// <summary>
///     DTO: Request to update existing Equipment.
///     This is the JSON the client sends in PUT.
/// </summary>
public record UpdateEquipmentResource(
    string Name,
    string Code,
    int PowerWatts,
    bool IsPoweredOn,
    string ActiveStatus,
    string? Notes,
    string Status,
    string LocationName,
    string LocationAddress,
    string? Image
);