namespace coolgym_webapi.Contexts.Equipments.Interfaces.REST.Resources;

/// <summary>
///     DTO: Request para crear un nuevo Equipment
///     Este es el JSON que el cliente envía en el POST
/// </summary>
public record CreateEquipmentResource(
    string Name,
    string Type,
    string Model,
    string Manufacturer,
    string SerialNumber,
    string Code,
    DateTime InstallationDate,
    int PowerWatts,
    string LocationName,
    string LocationAddress,
    string? Image
);