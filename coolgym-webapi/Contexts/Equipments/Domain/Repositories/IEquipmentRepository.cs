using coolgym_webapi.Contexts.Equipments.Domain.Model.Entities;
using coolgym_webapi.Contexts.Shared.Domain.Repositories;

namespace coolgym_webapi.Contexts.Equipments.Domain.Repositories;

/// <summary>
///     Specific repository for Equipment entity
///     Extends IBaseRepository with additional specialized methods
/// </summary>
public interface IEquipmentRepository : IBaseRepository<Equipment>
{
    Task<Equipment?> FindBySerialNumberAsync(string serialNumber);

    Task<IEnumerable<Equipment>> FindByTypeAsync(string type);

    Task<IEnumerable<Equipment>> FindByStatusAsync(string status);

    Task<IEnumerable<Equipment>> FindActiveEquipmentAsync();

    Task<bool> ExistsBySerialNumberAsync(string serialNumber);
}