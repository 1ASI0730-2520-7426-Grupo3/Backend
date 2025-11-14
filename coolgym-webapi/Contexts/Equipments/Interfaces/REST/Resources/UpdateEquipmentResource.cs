namespace coolgym_webapi.Contexts.Equipments.Interfaces.REST.Resources;

/// <summary>
///     DTO: Request para actualizar un Equipment existente.
///     Este es el JSON que el cliente envía en el PUT.
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