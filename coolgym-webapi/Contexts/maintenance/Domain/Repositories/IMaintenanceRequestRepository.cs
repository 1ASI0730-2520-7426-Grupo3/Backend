using coolgym_webapi.Contexts.maintenance.Domain.Model.Entities;
using coolgym_webapi.Contexts.maintenance.Domain.Queries;
using coolgym_webapi.Contexts.Shared.Domain.Repositories;

namespace coolgym_webapi.Contexts.maintenance.Domain.Repositories;

public interface IMaintenanceRequestRepository : IBaseRepository<MaintenanceRequest>
{
    Task<IEnumerable<MaintenanceRequest>> FindByStatusAsync(string status);
    
    Task<MaintenanceRequest?> FindByEquipmentIdAsync(int equipmentId);
}