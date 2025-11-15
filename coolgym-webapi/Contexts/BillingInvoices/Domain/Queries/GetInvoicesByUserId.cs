namespace coolgym_webapi.Contexts.BillingInvoices.Domain.Queries;

/// <summary>
///     Query to get all invoices for a specific user
/// </summary>
public record GetInvoicesByUserId(int UserId);