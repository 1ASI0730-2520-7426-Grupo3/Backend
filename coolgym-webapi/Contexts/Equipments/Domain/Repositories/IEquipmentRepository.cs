using coolgym_webapi.Contexts.Equipments.Domain.Model.Entities;
using coolgym_webapi.Contexts.Shared.Domain.Repositories;

namespace coolgym_webapi.Contexts.Equipments.Domain.Repositories;

/// <summary>
///     Repositorio específico para la entidad Equipment
///     Extiende IBaseRepository con métodos adicionales especializados
/// </summary>
public interface IEquipmentRepository : IBaseRepository<Equipment>
{
    Task<Equipment?> FindBySerialNumberAsync(string serialNumber);

    Task<IEnumerable<Equipment>> FindByTypeAsync(string type);

    Task<IEnumerable<Equipment>> FindByStatusAsync(string status);

    Task<IEnumerable<Equipment>> FindActiveEquipmentAsync();

    Task<bool> ExistsBySerialNumberAsync(string serialNumber);
}