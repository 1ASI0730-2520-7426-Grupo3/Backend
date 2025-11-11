namespace coolgym_webapi.Contexts.BillingInvoices.Interfaces.REST.Resources;

/// <summary>
/// Request DTO for creating a new billing invoice
/// </summary>
public record CreateInvoiceResource(
    int UserId,
    string CompanyName,
    decimal Amount,
    string Currency,
    string Status,           // "paid", "pending", or "cancelled"
    string IssuedAt,        // ISO 8601 format: "yyyy-MM-dd"
    string? PaidAt = null   // ISO 8601 format: "yyyy-MM-dd" (optional)
);
