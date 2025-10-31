namespace coolgym_webapi.Contexts.Equipments.Domain.Queries;

/// <summary>
/// Consulta para OBTENER UN equipo por su ID
/// Se usa en: GET /api/equipments/{id}
/// </summary>
public record GetEquipmentById(int Id);