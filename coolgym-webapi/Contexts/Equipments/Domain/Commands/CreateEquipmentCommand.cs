namespace coolgym_webapi.Contexts.Equipments.Domain.Commands;

/// <summary>
/// Comando para crear un nuevo equipo
/// Representa los datos necesarios para la operación de escritura
/// </summary>
public record CreateEquipmentCommand(
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