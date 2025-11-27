namespace coolgym_webapi.Contexts.BillingInvoices.Domain.Exceptions;

public class InvalidInvoiceDataException : Exception
{
    public InvalidInvoiceDataException(string message)
        : base($"Invalid invoice data: {message}")
    {
    }

    public static InvalidInvoiceDataException InvalidUserId(int userId)
    {
        return new InvalidInvoiceDataException($"User identifier must be positive. Value: {userId}.");
    }

    public static InvalidInvoiceDataException EmptyCompanyName()
    {
        return new InvalidInvoiceDataException("Company name is required.");
    }

    public static InvalidInvoiceDataException CompanyNameTooLong(int length, int maxLength)
    {
        return new InvalidInvoiceDataException($"Company name cannot exceed {maxLength} characters. Current length: {length}.");
    }

    public static InvalidInvoiceDataException InvalidMoney(decimal amount, string currency)
    {
        return new InvalidInvoiceDataException($"Invalid amount or currency. Amount: {amount}, Currency: '{currency}'.");
    }

    public static InvalidInvoiceDataException InvalidStatus(string status)
    {
        return new InvalidInvoiceDataException($"Status '{status}' is invalid. Allowed values: 'paid', 'pending', 'cancelled'.");
    }

    public static InvalidInvoiceDataException PaidDateBeforeIssued(DateTime issuedAt, DateTime paidAt)
    {
        return new InvalidInvoiceDataException($"Paid date {paidAt:O} cannot be before issued date {issuedAt:O}.");
    }

    public static InvalidInvoiceDataException MissingPaidDate()
    {
        return new InvalidInvoiceDataException("Paid invoices must have a paid date.");
    }

    public static InvalidInvoiceDataException PaidDateForNonPaid()
    {
        return new InvalidInvoiceDataException("Only invoices with status 'paid' can have a paid date.");
    }
}