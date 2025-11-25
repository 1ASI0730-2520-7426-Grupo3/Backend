namespace coolgym_webapi.Contexts.Equipments.Domain.Queries;

/// <summary>
///     Query to GET ONE equipment by ID
///     Used in: GET /api/equipments/{id}
/// </summary>
public record GetEquipmentById(int Id);