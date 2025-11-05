namespace coolgym_webapi.Contexts.Equipments.Domain.Model.ValueObjects;

public record MaintenanceInfo(
    DateTime? LastMaintenanceDate,
    DateTime? NextMaintenanceDate
);