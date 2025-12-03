namespace coolgym_webapi.Contexts.maintenance.Domain.Commands;

public record CreateMaintenanceRequestCommand(
    int EquipmentId,
    DateTime SelectedDate,
    string Observation,
    int RequestedByUserId,
    int? AssignedToProviderId = null
);