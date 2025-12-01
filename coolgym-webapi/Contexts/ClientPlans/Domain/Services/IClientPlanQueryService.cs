using coolgym_webapi.Contexts.ClientPlans.Domain.Model.Entities;
using coolgym_webapi.Contexts.ClientPlans.Domain.Queries;

namespace coolgym_webapi.Contexts.ClientPlans.Domain.Services;

/// <summary>
/// Query service interface for client plans
/// </summary>
public interface IClientPlanQueryService
{
    Task<IEnumerable<ClientPlan>> Handle(GetAllClientPlans query);
    Task<ClientPlan?> Handle(GetClientPlanById query);
}
