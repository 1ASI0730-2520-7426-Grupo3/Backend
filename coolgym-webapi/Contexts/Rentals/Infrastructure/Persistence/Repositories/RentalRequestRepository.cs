using coolgym_webapi.Contexts.Rentals.Domain.Model.Entities;
using coolgym_webapi.Contexts.Rentals.Domain.Repositories;
using coolgym_webapi.Contexts.Shared.Infrastructure.Persistence.Configuration;
using coolgym_webapi.Contexts.Shared.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace coolgym_webapi.Contexts.Rentals.Infrastructure.Persistence.Repositories;

public class RentalRequestRepository(CoolgymContext context)
    : BaseRepository<RentalRequest>(context), IRentalRequestRepository
{
    private readonly CoolgymContext _context = context;

    public async Task<IEnumerable<RentalRequest>> FindByClientIdAsync(int clientId)
    {
        return await _context.Set<RentalRequest>()
            .Include(rr => rr.Equipment)
            .Include(rr => rr.Client)
            .Include(rr => rr.Provider)
            .Where(rr => rr.ClientId == clientId && rr.IsDeleted == 0)
            .OrderByDescending(rr => rr.RequestDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<RentalRequest>> FindByStatusAsync(string status)
    {
        return await _context.Set<RentalRequest>()
            .Include(rr => rr.Equipment)
            .Include(rr => rr.Client)
            .Include(rr => rr.Provider)
            .Where(rr => rr.Status == status && rr.IsDeleted == 0)
            .OrderByDescending(rr => rr.RequestDate)
            .ToListAsync();
    }

    public override async Task<RentalRequest?> FindByIdAsync(int id)
    {
        return await _context.Set<RentalRequest>()
            .Include(rr => rr.Equipment)
            .Include(rr => rr.Client)
            .Include(rr => rr.Provider)
            .FirstOrDefaultAsync(rr => rr.Id == id && rr.IsDeleted == 0);
    }

    public override async Task<IEnumerable<RentalRequest>> ListAsync()
    {
        return await _context.Set<RentalRequest>()
            .Include(rr => rr.Equipment)
            .Include(rr => rr.Client)
            .Include(rr => rr.Provider)
            .Where(rr => rr.IsDeleted == 0)
            .OrderByDescending(rr => rr.RequestDate)
            .ToListAsync();
    }
}
