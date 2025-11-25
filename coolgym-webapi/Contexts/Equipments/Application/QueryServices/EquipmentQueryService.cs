using coolgym_webapi.Contexts.Equipments.Domain.Model.Entities;
using coolgym_webapi.Contexts.Equipments.Domain.Queries;
using coolgym_webapi.Contexts.Equipments.Domain.Repositories;
using coolgym_webapi.Contexts.Equipments.Domain.Services;

namespace coolgym_webapi.Contexts.Equipments.Application.QueryServices;

/// <summary>
///     Application service for Equipment queries (Read operations)
///     Implements logic for GET, LIST, FILTER
/// </summary>
public class EquipmentQueryService(IEquipmentRepository equipmentRepository)
    : IEquipmentQueryService
{
    /// <summary>
    ///     Handles query to get all equipment
    /// </summary>
    /// <param name="query">GetAllEquipment query</param>
    /// <returns>List of all equipment</returns>
    public async Task<IEnumerable<Equipment>> Handle(GetAllEquipment query)
    {
        return await equipmentRepository.ListAsync();
    }

    /// <summary>
    ///     Handles query to get equipment by ID
    /// </summary>
    /// <param name="query">GetEquipmentById query with ID</param>
    /// <returns>Found equipment or null if not found</returns>
    public async Task<Equipment?> Handle(GetEquipmentById query)
    {
        return await equipmentRepository.FindByIdAsync(query.Id);
    }

    /// <summary>
    ///     Handles query to get equipment filtered by type
    /// </summary>
    /// <param name="query">GetEquipmentByType query with type to search</param>
    /// <returns>List of equipment of specified type</returns>
    public async Task<IEnumerable<Equipment>> Handle(GetEquipmentByType query)
    {
        return await equipmentRepository.FindByTypeAsync(query.Type);
    }

    /// <summary>
    ///     Handles query to get equipment filtered by status
    /// </summary>
    /// <param name="query">GetEquipmentByStatus query with status to search</param>
    /// <returns>List of equipment with specified status</returns>
    public async Task<IEnumerable<Equipment>> Handle(GetEquipmentByStatus query)
    {
        return await equipmentRepository.FindByStatusAsync(query.Status);
    }
}