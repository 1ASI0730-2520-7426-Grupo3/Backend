using coolgym_webapi.Contexts.BillingInvoices.Domain.Model.Entities;
using coolgym_webapi.Contexts.BillingInvoices.Domain.Repositories;
using coolgym_webapi.Contexts.Shared.Infrastructure.Persistence.Configuration;
using coolgym_webapi.Contexts.Shared.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace coolgym_webapi.Contexts.BillingInvoices.Infrastructure.Persistence.Repositories;

/// <summary>
/// Repository implementation for BillingInvoice using Entity Framework Core
/// </summary>
public class BillingInvoiceRepository(CoolgymContext context)
    : BaseRepository<BillingInvoice>(context), IBillingInvoiceRepository
{
    public async Task<IEnumerable<BillingInvoice>> FindByUserIdAsync(int userId)
    {
        // IMPORTANT: Always filter soft deletes (IsDeleted == 0)
        return await context.BillingInvoices
            .Where(i => i.IsDeleted == 0 && i.UserId == userId)
            .OrderByDescending(i => i.IssuedAt) // Most recent first
            .ToListAsync();
    }

    public async Task<IEnumerable<BillingInvoice>> FindByStatusAsync(string status)
    {
        return await context.BillingInvoices
            .Where(i => i.IsDeleted == 0 && i.Status.Value == status.ToLower())
            .OrderByDescending(i => i.IssuedAt)
            .ToListAsync();
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await context.BillingInvoices
            .AnyAsync(i => i.IsDeleted == 0 && i.Id == id);
    }
}
