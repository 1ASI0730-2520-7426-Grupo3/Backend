using coolgym_webapi.Contexts.maintenance.Domain.Model.Entities;
using coolgym_webapi.Contexts.maintenance.Domain.Repositories;
using coolgym_webapi.Contexts.Shared.Infrastructure.Persistence.Configuration;
using coolgym_webapi.Contexts.Shared.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace coolgym_webapi.Contexts.maintenance.Infrastructure.Persistence.Repositories;

public class MaintenanceRequestRepository(CoolgymContext context)
    : BaseRepository<MaintenanceRequest>(context), IMaintenanceRequestRepository
{
    public async Task<IEnumerable<MaintenanceRequest>> FindByStatusAsync(string status)
    {
        var normalizedStatus = status.ToLowerInvariant();
        return await context.MaintenanceRequests
            .Where(e => e.Status == normalizedStatus)
            .ToListAsync();
    }

    public async Task<MaintenanceRequest?> FindByEquipmentIdAsync(int equipmentId)
    {
        return await context.MaintenanceRequests
            .FirstOrDefaultAsync(e => e.EquipmentId == equipmentId);
    }
}