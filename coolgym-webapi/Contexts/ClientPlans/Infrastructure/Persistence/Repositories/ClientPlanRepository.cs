using coolgym_webapi.Contexts.ClientPlans.Domain.Model.Entities;
using coolgym_webapi.Contexts.ClientPlans.Domain.Repositories;
using coolgym_webapi.Contexts.Shared.Infrastructure.Persistence.Configuration;
using coolgym_webapi.Contexts.Shared.Infrastructure.Repositories;

namespace coolgym_webapi.Contexts.ClientPlans.Infrastructure.Persistence.Repositories;

/// <summary>
///     Repository implementation for ClientPlan entity
/// </summary>
public class ClientPlanRepository(CoolgymContext context)
    : BaseRepository<ClientPlan>(context), IClientPlanRepository
{
}