namespace coolgym_webapi.Contexts.BillingInvoices.Domain.Exceptions;

public class InvoiceStatusTransitionException : Exception
{
    public InvoiceStatusTransitionException(string message)
        : base(message)
    {
    }

    public static InvoiceStatusTransitionException AlreadyPaid()
    {
        return new InvoiceStatusTransitionException("Invoice is already paid.");
    }

    public static InvoiceStatusTransitionException CannotCancelPaid()
    {
        return new InvoiceStatusTransitionException("Cannot cancel a paid invoice.");
    }
}