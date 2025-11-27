namespace coolgym_webapi.Contexts.BillingInvoices.Domain.Exceptions;

public class InvoiceNotFoundException : Exception
{
    public InvoiceNotFoundException(int id)
        : base($"Invoice with id '{id}' not found.")
    {
        InvoiceId = id;
    }

    public int InvoiceId { get; }
}