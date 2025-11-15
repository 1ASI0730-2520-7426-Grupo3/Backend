using coolgym_webapi.Contexts.BillingInvoices.Domain.Model.Entities;
using coolgym_webapi.Contexts.Shared.Domain.Repositories;

namespace coolgym_webapi.Contexts.BillingInvoices.Domain.Repositories;

/// <summary>
///     Repository interface for BillingInvoice aggregate
///     Extends base repository with specialized queries
/// </summary>
public interface IBillingInvoiceRepository : IBaseRepository<BillingInvoice>
{
    /// <summary>
    ///     Find all invoices for a specific user
    /// </summary>
    Task<IEnumerable<BillingInvoice>> FindByUserIdAsync(int userId);

    /// <summary>
    ///     Find invoices by status
    /// </summary>
    Task<IEnumerable<BillingInvoice>> FindByStatusAsync(string status);

    /// <summary>
    ///     Check if an invoice exists by ID
    /// </summary>
    Task<bool> ExistsAsync(int id);
}