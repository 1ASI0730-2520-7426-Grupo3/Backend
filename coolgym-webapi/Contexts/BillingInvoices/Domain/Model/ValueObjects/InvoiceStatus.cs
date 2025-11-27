namespace coolgym_webapi.Contexts.BillingInvoices.Domain.Model.ValueObjects;

/// <summary>
///     Represents the status of an invoice
///     Valid values: paid, pending, cancelled
/// </summary>
public record InvoiceStatus
{
    private InvoiceStatus(string value)
    {
        Value = value;
    }

    public string Value { get; init; }

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

    public bool IsPaid()
    {
        return Value == "paid";
    }

    public bool IsPending()
    {
        return Value == "pending";
    }

    public bool IsCancelled()
    {
        return Value == "cancelled";
    }

    public override string ToString()
    {
        return Value;
    }
}