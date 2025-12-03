namespace coolgym_webapi.Contexts.Equipments.Domain.Commands;

/// <summary>
///     Command to create new equipment
///     Represents data needed for the write operation
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
    string? Image,
    int? OwnerUserId
);