using coolgym_webapi.Contexts.maintenance.Domain.Commands;
using coolgym_webapi.Contexts.maintenance.Interfaces.REST.Resources;

namespace coolgym_webapi.Contexts.maintenance.Interfaces.REST.Transform;

public static class UpdateMaintenanceRequestStatusCommandFromResourceAssembler
{
    public static UpdateMaintenanceRequestStatusCommand ToCommandFromResource(int id,
        UpdateMaintenanceRequestStatusResource resource)
    {
        return new UpdateMaintenanceRequestStatusCommand(
            id,
            resource.Status
        );
    }
}