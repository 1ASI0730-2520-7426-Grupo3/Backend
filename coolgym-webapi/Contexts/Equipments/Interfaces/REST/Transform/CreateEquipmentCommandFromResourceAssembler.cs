using coolgym_webapi.Contexts.Equipments.Domain.Commands;
using coolgym_webapi.Contexts.Equipments.Interfaces.REST.Resources;

namespace coolgym_webapi.Contexts.Equipments.Interfaces.REST.Transform;

/// <summary>
///     Assembler: Converts Resource → Command (API → Domain)
///     Transforms HTTP request DTO into domain Command
/// </summary>
public static class CreateEquipmentCommandFromResourceAssembler
{
    /// <summary>
    ///     Converts CreateEquipmentResource into CreateEquipmentCommand
    /// </summary>
    public static CreateEquipmentCommand ToCommandFromResource(CreateEquipmentResource resource)
    {
        return new CreateEquipmentCommand(
            resource.Name,
            resource.Type,
            resource.Model,
            resource.Manufacturer,
            resource.SerialNumber,
            resource.Code,
            resource.InstallationDate,
            resource.PowerWatts,
            resource.LocationName,
            resource.LocationAddress,
            resource.Image
        );
    }
}