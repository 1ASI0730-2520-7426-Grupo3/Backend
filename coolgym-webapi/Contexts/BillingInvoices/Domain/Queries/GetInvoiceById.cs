namespace coolgym_webapi.Contexts.BillingInvoices.Domain.Queries;

/// <summary>
///     Query to get a specific invoice by ID
/// </summary>
public record GetInvoiceById(int InvoiceId);