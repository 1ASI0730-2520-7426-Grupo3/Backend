namespace coolgym_webapi.Contexts.BillingInvoices.Interfaces.REST.Resources;

/// <summary>
/// Request DTO for marking an invoice as paid
/// </summary>
public record MarkInvoiceAsPaidResource(
    string PaidAt  // ISO 8601 format: "yyyy-MM-dd"
);
