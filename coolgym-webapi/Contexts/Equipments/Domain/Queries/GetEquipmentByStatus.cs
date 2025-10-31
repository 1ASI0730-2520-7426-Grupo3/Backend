namespace coolgym_webapi.Contexts.Equipments.Domain.Queries;

/// <summary>
/// Consulta para OBTENER equipos por ESTADO (active, pending_maintenance, etc.)
/// Se usa en: GET /api/equipments/status/{status}
/// </summary>
public record GetEquipmentByStatus(string Status);