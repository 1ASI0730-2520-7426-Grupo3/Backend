using coolgym_webapi.Contexts.BillingInvoices.Domain.Model.ValueObjects;
using coolgym_webapi.Contexts.Shared.Domain.Model.Entities;

namespace coolgym_webapi.Contexts.BillingInvoices.Domain.Model.Entities;

/// <summary>
/// Billing Invoice aggregate root
/// Represents an invoice for a user's payment
/// </summary>
public class BillingInvoice : BaseEntity
{
    public int UserId { get; private set; }
    public string CompanyName { get; private set; }
    public Money Amount { get; private set; }
    public InvoiceStatus Status { get; private set; }
    public DateTime IssuedAt { get; private set; }
    public DateTime? PaidAt { get; private set; }

    // EF Core constructor
    protected BillingInvoice()
    {
        CompanyName = null!;
        Amount = null!;
        Status = null!;
    }

    public BillingInvoice(
        int userId,
        string companyName,
        decimal amount,
        string currency,
        string status,
        DateTime issuedAt,
        DateTime? paidAt = null)
    {
        // Validation
        if (userId <= 0)
            throw new ArgumentException("User ID must be positive", nameof(userId));

        if (string.IsNullOrWhiteSpace(companyName))
            throw new ArgumentException("Company name is required", nameof(companyName));

        var money = Money.Create(amount, currency);
        if (money == null)
            throw new ArgumentException("Invalid amount or currency");

        var invoiceStatus = InvoiceStatus.FromString(status);
        if (invoiceStatus == null)
            throw new ArgumentException($"Invalid status: {status}. Must be 'paid', 'pending', or 'cancelled'");

        // Business Rule: paidAt must be after issuedAt
        if (paidAt.HasValue && paidAt.Value < issuedAt)
            throw new ArgumentException("Paid date cannot be before issued date");

        // Business Rule: paidAt should only be set when status is 'paid'
        if (invoiceStatus.IsPaid() && !paidAt.HasValue)
            throw new ArgumentException("Paid invoices must have a paid date");

        if (!invoiceStatus.IsPaid() && paidAt.HasValue)
            throw new ArgumentException("Only paid invoices can have a paid date");

        UserId = userId;
        CompanyName = companyName;
        Amount = money;
        Status = invoiceStatus;
        IssuedAt = issuedAt;
        PaidAt = paidAt;
        CreatedDate = DateTime.UtcNow;
    }

    /// <summary>
    /// Mark invoice as paid
    /// </summary>
    public void MarkAsPaid(DateTime paidAt)
    {
        if (Status.IsPaid())
            throw new InvalidOperationException("Invoice is already paid");

        if (paidAt < IssuedAt)
            throw new ArgumentException("Paid date cannot be before issued date");

        Status = InvoiceStatus.Paid;
        PaidAt = paidAt;
        UpdatedDate = DateTime.UtcNow;
    }

    /// <summary>
    /// Cancel invoice
    /// </summary>
    public void Cancel()
    {
        if (Status.IsPaid())
            throw new InvalidOperationException("Cannot cancel a paid invoice");

        Status = InvoiceStatus.Cancelled;
        UpdatedDate = DateTime.UtcNow;
    }
}
