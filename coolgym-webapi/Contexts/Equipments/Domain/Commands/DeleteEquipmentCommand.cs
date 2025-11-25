namespace coolgym_webapi.Contexts.Equipments.Domain.Commands;

/// <summary>
///     Command to DELETE equipment
///     Used in: DELETE /api/equipments/{id}
/// </summary>
public record DeleteEquipmentCommand(int Id);