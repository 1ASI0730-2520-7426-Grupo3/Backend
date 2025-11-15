using coolgym_webapi.Contexts.maintenance.Domain.Model.Entities;
using coolgym_webapi.Contexts.maintenance.Interfaces.REST.Resources;

namespace coolgym_webapi.Contexts.maintenance.Interfaces.REST.Transform;

public static class MaintenanceRequestResourceFromEntityAssembler
{
    public static MaintenanceRequestResource ToResourceFromEntity(MaintenanceRequest entity)
    {
        return new MaintenanceRequestResource(
            entity.Id,
            entity.EquipmentId,
            entity.SelectedDate,
            entity.Observation,
            entity.Status
        );
    }

    public static IEnumerable<MaintenanceRequestResource> ToResourceFromEntity(IEnumerable<MaintenanceRequest> entities)
    {
        return entities.Select(ToResourceFromEntity);
    }
}