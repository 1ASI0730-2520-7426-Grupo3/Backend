using coolgym_webapi.Contexts.BillingInvoices.Domain.Model.Entities;
using coolgym_webapi.Contexts.BillingInvoices.Domain.Queries;

namespace coolgym_webapi.Contexts.BillingInvoices.Domain.Services;

/// <summary>
/// Query service interface for billing invoice read operations
/// </summary>
public interface IInvoiceQueryService
{
    /// <summary>
    /// Get all invoices for a specific user
    /// </summary>
    Task<IEnumerable<BillingInvoice>> Handle(GetInvoicesByUserId query);

    /// <summary>
    /// Get a specific invoice by ID
    /// </summary>
    Task<BillingInvoice?> Handle(GetInvoiceById query);

    /// <summary>
    /// Get all invoices (for admin)
    /// </summary>
    Task<IEnumerable<BillingInvoice>> Handle(GetAllInvoices query);
}
