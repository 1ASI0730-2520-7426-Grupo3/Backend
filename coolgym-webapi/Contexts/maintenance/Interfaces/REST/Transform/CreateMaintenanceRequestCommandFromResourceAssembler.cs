using coolgym_webapi.Contexts.maintenance.Domain.Commands;
using coolgym_webapi.Contexts.maintenance.Interfaces.REST.Resources;

namespace coolgym_webapi.Contexts.maintenance.Interfaces.REST.Transform;

public static class CreateMaintenanceRequestCommandFromResourceAssembler
{
    public static CreateMaintenanceRequestCommand ToCommandFromResource(
        CreateMaintenanceRequestResource resource,
        int requestedByUserId,
        int? assignedToProviderId = null)
    {
        return new CreateMaintenanceRequestCommand(
            resource.EquipmentId,
            resource.SelectedDate,
            resource.Observation,
            requestedByUserId,
            assignedToProviderId
        );
    }
}