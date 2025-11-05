using coolgym_webapi.Contexts.Equipments.Domain.Model.Entities;
using coolgym_webapi.Contexts.Equipments.Domain.Queries;
using coolgym_webapi.Contexts.Equipments.Domain.Repositories;
using coolgym_webapi.Contexts.Equipments.Domain.Services;

namespace coolgym_webapi.Contexts.Equipments.Application.QueryServices;

/// <summary>
///     Servicio de aplicación para consultas de Equipment (Operaciones de lectura)
///     Implementa la lógica para GET, LIST, FILTER
/// </summary>
public class EquipmentQueryService : IEquipmentQueryService
{
    private readonly IEquipmentRepository _equipmentRepository;

    public EquipmentQueryService(IEquipmentRepository equipmentRepository)
    {
        _equipmentRepository = equipmentRepository;
    }

    /// <summary>
    ///     Maneja la consulta para obtener todos los equipos
    /// </summary>
    /// <param name="query">Consulta GetAllEquipment</param>
    /// <returns>Lista de todos los equipos</returns>
    public async Task<IEnumerable<Equipment>> Handle(GetAllEquipment query)
    {
        return await _equipmentRepository.ListAsync();
    }

    /// <summary>
    ///     Maneja la consulta para obtener un equipo por su ID
    /// </summary>
    /// <param name="query">Consulta GetEquipmentById con el ID</param>
    /// <returns>El equipo encontrado o null si no existe</returns>
    public async Task<Equipment?> Handle(GetEquipmentById query)
    {
        return await _equipmentRepository.FindByIdAsync(query.Id);
    }

    /// <summary>
    ///     Maneja la consulta para obtener equipos filtrados por tipo
    /// </summary>
    /// <param name="query">Consulta GetEquipmentByType con el tipo a buscar</param>
    /// <returns>Lista de equipos del tipo especificado</returns>
    public async Task<IEnumerable<Equipment>> Handle(GetEquipmentByType query)
    {
        return await _equipmentRepository.FindByTypeAsync(query.Type);
    }

    /// <summary>
    ///     Maneja la consulta para obtener equipos filtrados por estado
    /// </summary>
    /// <param name="query">Consulta GetEquipmentByStatus con el estado a buscar</param>
    /// <returns>Lista de equipos con el estado especificado</returns>
    public async Task<IEnumerable<Equipment>> Handle(GetEquipmentByStatus query)
    {
        return await _equipmentRepository.FindByStatusAsync(query.Status);
    }
}