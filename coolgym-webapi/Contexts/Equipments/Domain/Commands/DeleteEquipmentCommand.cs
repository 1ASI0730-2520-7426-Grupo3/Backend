namespace coolgym_webapi.Contexts.Equipments.Domain.Commands;

/// <summary>
/// Comando para ELIMINAR un equipo
/// Se usa en: DELETE /api/equipments/{id}
/// </summary>
public record DeleteEquipmentCommand(int Id);