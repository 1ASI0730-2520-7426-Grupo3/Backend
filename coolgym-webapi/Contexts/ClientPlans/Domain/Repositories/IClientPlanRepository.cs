using coolgym_webapi.Contexts.ClientPlans.Domain.Model.Entities;
using coolgym_webapi.Contexts.Shared.Domain.Repositories;

namespace coolgym_webapi.Contexts.ClientPlans.Domain.Repositories;

/// <summary>
/// Repository interface for ClientPlan entity
/// </summary>
public interface IClientPlanRepository : IBaseRepository<ClientPlan>
{
}
