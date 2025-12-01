using coolgym_webapi.Contexts.ClientPlans.Domain.Model.Entities;
using coolgym_webapi.Contexts.ClientPlans.Domain.Queries;
using coolgym_webapi.Contexts.ClientPlans.Domain.Repositories;
using coolgym_webapi.Contexts.ClientPlans.Domain.Services;

namespace coolgym_webapi.Contexts.ClientPlans.Application.QueryServices;

/// <summary>
///     Query service for client plans
/// </summary>
public class ClientPlanQueryService(IClientPlanRepository clientPlanRepository) : IClientPlanQueryService
{
    public async Task<IEnumerable<ClientPlan>> Handle(GetAllClientPlans query)
    {
        return await clientPlanRepository.ListAsync();
    }

    public async Task<ClientPlan?> Handle(GetClientPlanById query)
    {
        return await clientPlanRepository.FindByIdAsync(query.Id);
    }
}