using coolgym_webapi.Contexts.maintenance.Domain.Model.Entities;
using coolgym_webapi.Contexts.maintenance.Domain.Queries;
using coolgym_webapi.Contexts.maintenance.Domain.Repositories;
using coolgym_webapi.Contexts.maintenance.Domain.Services;

namespace coolgym_webapi.Contexts.maintenance.Application.QueryServices;

public class MaintenanceRequestQueryService(IMaintenanceRequestRepository maintenanceRequestRepository)
    : IMaintenanceRequestQueryService
{
    public async Task<IEnumerable<MaintenanceRequest>> Handle(GetAllMaintenanceRequests query)
    {
        return await maintenanceRequestRepository.ListAsync();
    }

    public async Task<MaintenanceRequest?> Handle(GetMaintenanceRequestById query)
    {
        return await maintenanceRequestRepository.FindByIdAsync(query.Id);
    }

    public async Task<IEnumerable<MaintenanceRequest>> Handle(GetMaintenanceRequestsByStatus query)
    {
        return await maintenanceRequestRepository.FindByStatusAsync(query.Status);
    }
}