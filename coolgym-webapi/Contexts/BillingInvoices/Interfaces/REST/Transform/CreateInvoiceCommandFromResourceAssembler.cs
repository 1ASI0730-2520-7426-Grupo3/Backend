using coolgym_webapi.Contexts.BillingInvoices.Domain.Commands;
using coolgym_webapi.Contexts.BillingInvoices.Interfaces.REST.Resources;

namespace coolgym_webapi.Contexts.BillingInvoices.Interfaces.REST.Transform;

/// <summary>
///     Assembler to transform CreateInvoiceResource to CreateInvoiceCommand
/// </summary>
public static class CreateInvoiceCommandFromResourceAssembler
{
    public static CreateInvoiceCommand ToCommandFromResource(CreateInvoiceResource resource)
    {
        return new CreateInvoiceCommand(
            resource.UserId,
            resource.CompanyName,
            resource.Amount,
            resource.Currency,
            resource.Status,
            DateTime.Parse(resource.IssuedAt),
            resource.PaidAt != null ? DateTime.Parse(resource.PaidAt) : null
        );
    }
}