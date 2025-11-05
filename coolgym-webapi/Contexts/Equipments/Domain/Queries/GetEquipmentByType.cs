namespace coolgym_webapi.Contexts.Equipments.Domain.Queries;

/// <summary>
///     Consulta para OBTENER equipos por TIPO (treadmill, bike, etc.)
///     Se usa en: GET /api/equipments/type/{type}
/// </summary>
public record GetEquipmentByType(string Type);