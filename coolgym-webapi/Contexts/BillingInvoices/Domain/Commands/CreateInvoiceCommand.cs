namespace coolgym_webapi.Contexts.BillingInvoices.Domain.Commands;

/// <summary>
///     Command to create a new billing invoice
/// </summary>
public record CreateInvoiceCommand(
    int UserId,
    string CompanyName,
    decimal Amount,
    string Currency,
    string Status,
    DateTime IssuedAt,
    DateTime? PaidAt = null
);