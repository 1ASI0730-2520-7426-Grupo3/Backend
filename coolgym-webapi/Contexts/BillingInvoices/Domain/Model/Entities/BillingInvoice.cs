using coolgym_webapi.Contexts.BillingInvoices.Domain.Exceptions;
using coolgym_webapi.Contexts.BillingInvoices.Domain.Model.ValueObjects;
using coolgym_webapi.Contexts.Shared.Domain.Model.Entities;

namespace coolgym_webapi.Contexts.BillingInvoices.Domain.Model.Entities;

/// <summary>
/// Billing invoice aggregate root. Represents a payment invoice for a user.
/// </summary>
public class BillingInvoice : BaseEntity
{
    private const int MaxCompanyNameLength = 255;

    public int UserId { get; private set; }
    public string CompanyName { get; private set; }
    public Money Amount { get; private set; }
    public InvoiceStatus Status { get; private set; }
    public DateTime IssuedAt { get; private set; }
    public DateTime? PaidAt { get; private set; }

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
        if (userId <= 0)
            throw InvalidInvoiceDataException.InvalidUserId(userId);

        if (string.IsNullOrWhiteSpace(companyName))
            throw InvalidInvoiceDataException.EmptyCompanyName();

        if (companyName.Length > MaxCompanyNameLength)
            throw InvalidInvoiceDataException.CompanyNameTooLong(companyName.Length, MaxCompanyNameLength);

        var money = Money.Create(amount, currency);
        if (money == null)
            throw InvalidInvoiceDataException.InvalidMoney(amount, currency);

        var invoiceStatus = InvoiceStatus.FromString(status);
        if (invoiceStatus == null)
            throw InvalidInvoiceDataException.InvalidStatus(status);

        // Business Rule: paidAt must be after issuedAt
        if (paidAt.HasValue && paidAt.Value < issuedAt)
            throw InvalidInvoiceDataException.PaidDateBeforeIssued(issuedAt, paidAt.Value);

        // Business Rule: paidAt should only be set when status is 'paid'
        if (invoiceStatus.IsPaid() && !paidAt.HasValue)
            throw InvalidInvoiceDataException.MissingPaidDate();

        if (!invoiceStatus.IsPaid() && paidAt.HasValue)
            throw InvalidInvoiceDataException.PaidDateForNonPaid();

        UserId = userId;
        CompanyName = companyName.Trim();
        Amount = money;
        Status = invoiceStatus;
        IssuedAt = issuedAt;
        PaidAt = paidAt;
        CreatedDate = DateTime.UtcNow;
    }

    /// <summary>
    /// Marks invoice as paid using the given paid date.
    /// </summary>
    public void MarkAsPaid(DateTime paidAt)
    {
        if (Status.IsPaid())
            throw InvoiceStatusTransitionException.AlreadyPaid();

        if (paidAt < IssuedAt)
            throw InvalidInvoiceDataException.PaidDateBeforeIssued(IssuedAt, paidAt);

        Status = InvoiceStatus.Paid;
        PaidAt = paidAt;
        UpdatedDate = DateTime.UtcNow;
    }

    /// <summary>
    /// Cancels the invoice if it is not already paid.
    /// </summary>
    public void Cancel()
    {
        if (Status.IsPaid())
            throw InvoiceStatusTransitionException.CannotCancelPaid();

        Status = InvoiceStatus.Cancelled;
        UpdatedDate = DateTime.UtcNow;
    }
}
