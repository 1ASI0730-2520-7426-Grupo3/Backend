namespace coolgym_webapi.Contexts.Equipments.Domain.Commands;

/// <summary>
///     Comando para ACTUALIZAR un equipo existente
///     Se usa en: PUT /api/equipments/{id}
///     Actualiza datos básicos y ubicación (lo más común)
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