using coolgym_webapi.Contexts.Equipments.Domain.Model.Entities;
using coolgym_webapi.Contexts.Equipments.Interfaces.REST.Resources;

namespace coolgym_webapi.Contexts.Equipments.Interfaces.REST.Transform;

/// <summary>
///     Assembler: Convierte Entity → Resource (Domain → API)
///     Transforma la entidad de dominio en un DTO para la respuesta HTTP
/// </summary>
public static class EquipmentResourceFromEntityAssembler
{
    /// <summary>
    ///     Convierte una entidad Equipment en un EquipmentResource
    /// </summary>
    public static EquipmentResource ToResourceFromEntity(Equipment entity)
    {
        return new EquipmentResource(
            entity.Id,
            entity.Name,
            entity.Type,
            entity.Model,
            entity.Manufacturer,
            entity.Code,
            entity.SerialNumber,
            entity.InstallationDate,
            entity.PowerWatts,
            entity.Status,
            entity.IsPoweredOn,
            entity.ActiveStatus,
            entity.Notes,
            entity.Image,
            new LocationResource(entity.Location.Name, entity.Location.Address),
            new UsageStatsResource(
                entity.Usage.TotalMinutes,
                entity.Usage.TodayMinutes,
                entity.Usage.CaloriesToday
            ),
            new ControlSettingsResource(
                entity.Controls.Power,
                entity.Controls.CurrentLevel,
                entity.Controls.SetLevel,
                entity.Controls.MinLevelRange,
                entity.Controls.MaxLevelRange,
                entity.Controls.Status
            ),
            new MaintenanceInfoResource(
                entity.MaintenanceInfo.LastMaintenanceDate,
                entity.MaintenanceInfo.NextMaintenanceDate
            ),
            entity.CreatedDate,
            entity.UpdatedDate
        );
    }

    /// <summary>
    ///     Convierte una lista de entidades Equipment en una lista de EquipmentResource
    /// </summary>
    public static IEnumerable<EquipmentResource> ToResourceFromEntity(IEnumerable<Equipment> entities)
    {
        return entities.Select(ToResourceFromEntity);
    }
}