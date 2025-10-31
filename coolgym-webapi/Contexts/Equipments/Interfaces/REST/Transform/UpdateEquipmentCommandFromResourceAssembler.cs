using coolgym_webapi.Contexts.Equipments.Domain.Commands;
using coolgym_webapi.Contexts.Equipments.Interfaces.REST.Resources;

namespace coolgym_webapi.Contexts.Equipments.Interfaces.REST.Transform;

/// <summary>
/// Assembler: Convierte UpdateEquipmentResource en UpdateEquipmentCommand
/// </summary>
public static class UpdateEquipmentCommandFromResourceAssembler
{
    /// <summary>
    /// Convierte un UpdateEquipmentResource en un UpdateEquipmentCommand
    /// </summary>
    public static UpdateEquipmentCommand ToCommandFromResource(int id, UpdateEquipmentResource resource)
    {
        return new UpdateEquipmentCommand(
            id,
            resource.Name,
            resource.Code,
            resource.PowerWatts,
            resource.IsPoweredOn,
            resource.ActiveStatus,
            resource.Notes,
            resource.Status,
            resource.LocationName,
            resource.LocationAddress,
            Image: resource.Image
        );
    }
}