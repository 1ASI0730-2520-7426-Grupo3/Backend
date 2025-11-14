namespace coolgym_webapi.Contexts.maintenance.Interfaces.REST.Resources;

public record MaintenanceRequestResource(
    int Id,
    int EquipmentId,
    DateTime SelectedDate,
    string Observation,
    string Status
    );