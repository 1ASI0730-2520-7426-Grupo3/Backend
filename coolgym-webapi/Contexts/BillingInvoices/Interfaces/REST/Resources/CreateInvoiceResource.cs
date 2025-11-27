namespace coolgym_webapi.Contexts.BillingInvoices.Interfaces.REST.Resources;

/// <summary>
/// Request DTO for creating a new billing invoice
/// </summary>
public record CreateInvoiceResource(
    int UserId,
    string CompanyName,
    decimal Amount,
    string Currency,
    string Status,
    string IssuedAt,
    string? PaidAt = null,
    int? MaintenanceRequestId = null);