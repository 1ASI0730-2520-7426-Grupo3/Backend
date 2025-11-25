namespace coolgym_webapi.Contexts.Equipments.Domain.Queries;

/// <summary>
///     Query to GET equipment by STATUS (active, pending_maintenance, etc.)
///     Used in: GET /api/equipments/status/{status}
/// </summary>
public record GetEquipmentByStatus(string Status);