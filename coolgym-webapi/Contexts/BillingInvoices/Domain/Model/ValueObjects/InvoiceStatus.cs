namespace coolgym_webapi.Contexts.BillingInvoices.Domain.Model.ValueObjects;

/// <summary>
/// Represents the status of an invoice
/// Valid values: paid, pending, cancelled
/// </summary>
public record InvoiceStatus
{
    public string Value { get; init; }

    private InvoiceStatus(string value)
    {
        Value = value;
    }

    // Valid status values
    public static InvoiceStatus Paid => new("paid");
    public static InvoiceStatus Pending => new("pending");
    public static InvoiceStatus Cancelled => new("cancelled");

    public static InvoiceStatus? FromString(string? status)
    {
        if (string.IsNullOrWhiteSpace(status)) return null;

        return status.ToLower() switch
        {
            "paid" => Paid,
            "pending" => Pending,
            "cancelled" => Cancelled,
            _ => null
        };
    }

    public bool IsPaid() => Value == "paid";
    public bool IsPending() => Value == "pending";
    public bool IsCancelled() => Value == "cancelled";

    public override string ToString() => Value;
}
