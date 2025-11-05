using coolgym_webapi.Contexts.Equipments.Domain.Commands;
using coolgym_webapi.Contexts.Equipments.Interfaces.REST.Resources;

namespace coolgym_webapi.Contexts.Equipments.Interfaces.REST.Transform;

/// <summary>
///     Assembler: Convierte Resource → Command (API → Domain)
///     Transforma el DTO del request HTTP en un Command de dominio
/// </summary>
public static class CreateEquipmentCommandFromResourceAssembler
{
    /// <summary>
    ///     Convierte un CreateEquipmentResource en un CreateEquipmentCommand
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