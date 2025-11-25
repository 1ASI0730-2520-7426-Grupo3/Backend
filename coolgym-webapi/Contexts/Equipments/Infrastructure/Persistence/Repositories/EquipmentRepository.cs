using coolgym_webapi.Contexts.Equipments.Domain.Model.Entities;
using coolgym_webapi.Contexts.Equipments.Domain.Repositories;
using coolgym_webapi.Contexts.Shared.Infrastructure.Persistence.Configuration;
using coolgym_webapi.Contexts.Shared.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace coolgym_webapi.Contexts.Equipments.Infrastructure.Persistence.Repositories;

/// <summary>
///     Equipment repository implementation
///     Inherits from BaseRepository and adds specialized methods
/// </summary>
public class EquipmentRepository(CoolgymContext context)
    : BaseRepository<Equipment>(context), IEquipmentRepository
{
    /// <summary>
    ///     Finds equipment by unique serial number
    /// </summary>
    public async Task<Equipment?> FindBySerialNumberAsync(string serialNumber)
    {
        return await context.Equipments
            .Where(e => e.IsDeleted == 0)
            .FirstOrDefaultAsync(e => e.SerialNumber == serialNumber);
    }

    /// <summary>
    ///     Gets all equipment of a specific type (treadmill, bike, etc.)
    /// </summary>
    public async Task<IEnumerable<Equipment>> FindByTypeAsync(string type)
    {
        return await context.Equipments
            .Where(e => e.Type == type && e.IsDeleted == 0)
            .ToListAsync();
    }

    /// <summary>
    ///     Gets all equipment with a specific status (active, pending_maintenance, inactive)
    /// </summary>
    public async Task<IEnumerable<Equipment>> FindByStatusAsync(string status)
    {
        return await context.Equipments
            .Where(e => e.Status == status && e.IsDeleted == 0)
            .ToListAsync();
    }

    /// <summary>
    ///     Gets only active equipment
    /// </summary>
    public async Task<IEnumerable<Equipment>> FindActiveEquipmentAsync()
    {
        return await context.Equipments
            .Where(e => e.Status == "active" && e.IsDeleted == 0)
            .ToListAsync();
    }

    /// <summary>
    ///     Checks if equipment exists with given serial number
    ///     Useful for validations before creating new equipment
    /// </summary>
    public async Task<bool> ExistsBySerialNumberAsync(string serialNumber)
    {
        return await context.Equipments
            .Where(e => e.IsDeleted == 0)
            .AnyAsync(e => e.SerialNumber == serialNumber);
    }
}