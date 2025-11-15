using coolgym_webapi.Contexts.RentalCatalog.Domain.Model.Entities;
using coolgym_webapi.Contexts.RentalCatalog.Domain.Repositories;
using coolgym_webapi.Contexts.Shared.Infrastructure.Persistence.Configuration;
using coolgym_webapi.Contexts.Shared.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace coolgym_webapi.Contexts.RentalCatalog.Infrastructure.Persistence.Repositories;

public class RentalItemRepository : BaseRepository<RentalItem>, IRentalItemRepository
{
    // 👉 Campo propio, porque la base no expone Context/_context
    private readonly CoolgymContext _context;

    public RentalItemRepository(CoolgymContext context) : base(context)
    {
        _context = context;
    }

    public async Task<IEnumerable<RentalItem>> FindByTypeAsync(string type) =>
        await _context.Set<RentalItem>()
            .Where(x => x.IsDeleted == 0 && x.Type == type)
            .ToListAsync();

    public async Task<IEnumerable<RentalItem>> FindAvailableAsync() =>
        await _context.Set<RentalItem>()
            .Where(x => x.IsDeleted == 0 && x.IsAvailable)
            .ToListAsync();

    public Task<bool> ExistsByNameAndModelAsync(string name, string model) =>
        _context.Set<RentalItem>()
            .AnyAsync(x => x.IsDeleted == 0 && x.Name == name && x.Model == model);
}