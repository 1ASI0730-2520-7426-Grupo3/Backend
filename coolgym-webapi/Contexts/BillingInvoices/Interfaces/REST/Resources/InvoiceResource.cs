namespace coolgym_webapi.Contexts.BillingInvoices.Interfaces.REST.Resources;

/// <summary>
///     Resource (DTO) representing a billing invoice in API responses
///     Matches the frontend JSON format
/// </summary>
public record InvoiceResource(
    int Id,
    int UserId,
    string CompanyName,
    decimal Amount,
    string Currency,
    string Status,
    string IssuedAt, // ISO 8601 date format (yyyy-MM-dd)
    string? PaidAt // ISO 8601 date format (yyyy-MM-dd) or null
);