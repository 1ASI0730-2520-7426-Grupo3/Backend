namespace coolgym_webapi.Contexts.maintenance.Interfaces.REST.Resources;

public record CreateMaintenanceRequestResource(
    int EquipmentId,
    DateTime SelectedDate,
    string Observation
);