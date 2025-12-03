using coolgym_webapi.Contexts.maintenance.Domain.Model.Entities;
using coolgym_webapi.Contexts.maintenance.Domain.Queries;
using coolgym_webapi.Contexts.maintenance.Domain.Repositories;
using coolgym_webapi.Contexts.maintenance.Domain.Services;
using coolgym_webapi.Contexts.maintenance.Interfaces.REST.Resources;
using coolgym_webapi.Contexts.Security.Domain.Infrastructure;

namespace coolgym_webapi.Contexts.maintenance.Application.QueryServices;

public class MaintenanceRequestQueryService(
    IMaintenanceRequestRepository maintenanceRequestRepository,
    IUserRepository userRepository)
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

    public async Task<IEnumerable<MaintenanceWithUserInfoResource>> Handle(GetMaintenanceRequestsByProviderId query)
    {
        // Get all maintenance requests for this provider
        var maintenanceRequests = await maintenanceRequestRepository.ListAsync();
        var providerRequests = maintenanceRequests
            .Where(mr => mr.AssignedToProviderId == query.ProviderId && mr.IsDeleted == 0)
            .OrderByDescending(mr => mr.CreatedDate)
            .ToList();

        var result = new List<MaintenanceWithUserInfoResource>();

        foreach (var request in providerRequests)
        {
            // Fetch user information separately (safe - no entity modification)
            var client = await userRepository.FindByIdAsync(request.RequestedByUserId);
            var provider = request.AssignedToProviderId.HasValue
                ? await userRepository.FindByIdAsync(request.AssignedToProviderId.Value)
                : null;

            result.Add(new MaintenanceWithUserInfoResource(
                request.Id,
                request.EquipmentId,
                request.RequestedByUserId,
                request.AssignedToProviderId,
                request.SelectedDate,
                request.Observation,
                request.Status,
                request.CreatedDate,
                client?.Name,
                client?.Email.Value,
                provider?.Name,
                provider?.Email.Value
            ));
        }

        return result;
    }
}