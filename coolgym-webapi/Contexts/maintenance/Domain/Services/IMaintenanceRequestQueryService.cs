using coolgym_webapi.Contexts.maintenance.Domain.Model.Entities;
using coolgym_webapi.Contexts.maintenance.Domain.Queries;

namespace coolgym_webapi.Contexts.maintenance.Domain.Services;

public interface IMaintenanceRequestQueryService
{
    Task<IEnumerable<MaintenanceRequest>> Handle(GetAllMaintenanceRequests query);

    Task<MaintenanceRequest?> Handle(GetMaintenanceRequestById query);

    Task<IEnumerable<MaintenanceRequest>> Handle(GetMaintenanceRequestsByStatus query);
}