namespace coolgym_webapi.Contexts.BillingInvoices.Domain.Commands;

/// <summary>
///     Command to delete (soft delete) an invoice
/// </summary>
public record DeleteInvoiceCommand(int InvoiceId);