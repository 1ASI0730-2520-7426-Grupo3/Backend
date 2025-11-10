using coolgym_webapi.Contexts.Equipments.Domain.Model.Entities;
using coolgym_webapi.Contexts.Equipments.Domain.Repositories;
using coolgym_webapi.Contexts.Shared.Infrastructure.Persistence.Configuration;
using coolgym_webapi.Contexts.Shared.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace coolgym_webapi.Contexts.Equipments.Infrastructure.Persistence.Repositories;

/// <summary>
///     Implementación del repositorio de Equipment
///     Hereda de BaseRepository y añade métodos especializados
/// </summary>
public class EquipmentRepository(CoolgymContext context) 
    : BaseRepository<Equipment>(context), IEquipmentRepository
{
    /// <summary>
    ///     Busca un equipo por su número de serie único
    /// </summary>
    public async Task<Equipment?> FindBySerialNumberAsync(string serialNumber)
    {
        return await context.Equipments
            .Where(e => e.IsDeleted == 0) 
            .FirstOrDefaultAsync(e => e.SerialNumber == serialNumber);
    }

    /// <summary>
    ///     Obtiene todos los equipos de un tipo específico (treadmill, bike, etc.)
    /// </summary>
    public async Task<IEnumerable<Equipment>> FindByTypeAsync(string type)
    {
        return await context.Equipments
            .Where(e => e.Type == type && e.IsDeleted == 0) 
            .ToListAsync();
    }

    /// <summary>
    ///     Obtiene todos los equipos con un estado específico (active, pending_maintenance, inactive)
    /// </summary>
    public async Task<IEnumerable<Equipment>> FindByStatusAsync(string status)
    {
        return await context.Equipments
            .Where(e => e.Status == status && e.IsDeleted == 0) 
            .ToListAsync();
    }

    /// <summary>
    ///     Obtiene solo los equipos activos
    /// </summary>
    public async Task<IEnumerable<Equipment>> FindActiveEquipmentAsync()
    {
        return await context.Equipments
            .Where(e => e.Status == "active" && e.IsDeleted == 0) 
            .ToListAsync();
    }

    /// <summary>
    ///     Verifica si existe un equipo con el número de serie dado
    ///     Útil para validaciones antes de crear nuevos equipos
    /// </summary>
    public async Task<bool> ExistsBySerialNumberAsync(string serialNumber)
    {
        return await context.Equipments
            .Where(e => e.IsDeleted == 0) 
            .AnyAsync(e => e.SerialNumber == serialNumber);
    }
}