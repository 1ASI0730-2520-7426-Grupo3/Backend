namespace coolgym_webapi.Contexts.maintenance.Interfaces.REST.Resources;

/// <summary>
/// DTO for maintenance request with user information
/// </summary>
public record MaintenanceWithUserInfoResource(
    int Id,
    int EquipmentId,
    int RequestedByUserId,
    int? AssignedToProviderId,
    DateTime SelectedDate,
    string Observation,
    string Status,
    DateTime CreatedDate,
    string? ClientName,
    string? ClientEmail,
    string? ProviderName,
    string? ProviderEmail
);
