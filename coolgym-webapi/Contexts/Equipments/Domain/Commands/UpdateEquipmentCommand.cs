namespace coolgym_webapi.Contexts.Equipments.Domain.Commands;

/// <summary>
///     Command to UPDATE existing equipment
///     Used in: PUT /api/equipments/{id}
///     Updates basic data and location (most common)
/// </summary>
public record UpdateEquipmentCommand(
    int Id,
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