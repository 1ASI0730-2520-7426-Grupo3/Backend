using coolgym_webapi.Contexts.Equipments.Domain.Model.Entities;
using coolgym_webapi.Contexts.Equipments.Domain.Queries;

namespace coolgym_webapi.Contexts.Equipments.Domain.Services;

/// <summary>
/// Interfaz del servicio de consultas (operaciones de lectura: GET, LIST, FILTER)
/// </summary>
public interface IEquipmentQueryService
{
    /// <summary>
    /// Obtiene todos los equipos registrados
    /// </summary>
    Task<IEnumerable<Equipment>> Handle(GetAllEquipment query);
    
    /// <summary>
    /// Obtiene un equipo específico por su ID
    /// </summary>
    Task<Equipment?> Handle(GetEquipmentById query);
    
    /// <summary>
    /// Obtiene equipos filtrados por tipo (treadmill, bike, etc.)
    /// </summary>
    Task<IEnumerable<Equipment>> Handle(GetEquipmentByType query);
    
    /// <summary>
    /// Obtiene equipos filtrados por estado (active, pending_maintenance, etc.)
    /// </summary>
    Task<IEnumerable<Equipment>> Handle(GetEquipmentByStatus query);
}