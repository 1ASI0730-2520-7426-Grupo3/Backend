namespace coolgym_webapi.Contexts.Equipments.Domain.Queries;

/// <summary>
///     Query to GET equipment by TYPE (treadmill, bike, etc.)
///     Used in: GET /api/equipments/type/{type}
/// </summary>
public record GetEquipmentByType(string Type);