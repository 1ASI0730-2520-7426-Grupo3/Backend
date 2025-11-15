namespace coolgym_webapi.Contexts.BillingInvoices.Domain.Commands;

/// <summary>
///     Command to mark an invoice as paid
/// </summary>
public record MarkInvoiceAsPaidCommand(
    int InvoiceId,
    DateTime PaidAt
);