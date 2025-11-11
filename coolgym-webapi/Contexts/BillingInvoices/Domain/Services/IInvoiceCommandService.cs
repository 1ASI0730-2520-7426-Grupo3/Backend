using coolgym_webapi.Contexts.BillingInvoices.Domain.Commands;
using coolgym_webapi.Contexts.BillingInvoices.Domain.Model.Entities;

namespace coolgym_webapi.Contexts.BillingInvoices.Domain.Services;

/// <summary>
/// Command service interface for billing invoice write operations
/// </summary>
public interface IInvoiceCommandService
{
    /// <summary>
    /// Create a new invoice
    /// </summary>
    Task<BillingInvoice> Handle(CreateInvoiceCommand command);

    /// <summary>
    /// Mark an invoice as paid
    /// </summary>
    Task<BillingInvoice> Handle(MarkInvoiceAsPaidCommand command);

    /// <summary>
    /// Delete (soft delete) an invoice
    /// </summary>
    Task<bool> Handle(DeleteInvoiceCommand command);
}
