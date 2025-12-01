using coolgym_webapi.Contexts.ClientPlans.Domain.Model.Entities;
using coolgym_webapi.Contexts.ClientPlans.Interfaces.REST.Resources;

namespace coolgym_webapi.Contexts.ClientPlans.Interfaces.REST.Transform;

/// <summary>
///     Assembler to transform ClientPlan entity to ClientPlanResource
/// </summary>
public static class ClientPlanResourceFromEntityAssembler
{
    public static ClientPlanResource ToResourceFromEntity(ClientPlan entity)
    {
        return new ClientPlanResource(
            entity.Id,
            entity.Name,
            entity.Description,
            entity.MonthlyPrice,
            entity.MaxEquipmentAccess,
            entity.HasMaintenanceSupport,
            entity.HasPrioritySupport
        );
    }
}