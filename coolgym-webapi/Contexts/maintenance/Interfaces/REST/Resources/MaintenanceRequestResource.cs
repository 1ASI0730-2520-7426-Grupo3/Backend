namespace coolgym_webapi.Contexts.maintenance.Interfaces.REST.Resources;

public record MaintenanceRequestResource(
    int Id,
    int EquipmentId,
    int RequestedByUserId,
    int? AssignedToProviderId,
    DateTime SelectedDate,
    string Observation,
    string Status,
    string? EquipmentName = null
);