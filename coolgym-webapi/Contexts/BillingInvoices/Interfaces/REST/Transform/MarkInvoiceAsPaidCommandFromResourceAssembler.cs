using coolgym_webapi.Contexts.BillingInvoices.Domain.Commands;
using coolgym_webapi.Contexts.BillingInvoices.Interfaces.REST.Resources;

namespace coolgym_webapi.Contexts.BillingInvoices.Interfaces.REST.Transform;

/// <summary>
/// Assembler to transform MarkInvoiceAsPaidResource to MarkInvoiceAsPaidCommand
/// </summary>
public static class MarkInvoiceAsPaidCommandFromResourceAssembler
{
    public static MarkInvoiceAsPaidCommand ToCommandFromResource(
        int invoiceId,
        MarkInvoiceAsPaidResource resource)
    {
        return new MarkInvoiceAsPaidCommand(
            InvoiceId: invoiceId,
            PaidAt: DateTime.Parse(resource.PaidAt)
        );
    }
}
