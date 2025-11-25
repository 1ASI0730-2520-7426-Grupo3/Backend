namespace coolgym_webapi.Contexts.Equipments.Interfaces.REST.Resources;

/// <summary>
///     DTO: Request to create new Equipment.
///     This is the JSON the client sends in POST.
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