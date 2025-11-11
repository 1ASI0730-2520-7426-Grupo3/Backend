using coolgym_webapi.Contexts.BillingInvoices.Domain.Commands;
using coolgym_webapi.Contexts.BillingInvoices.Interfaces.REST.Resources;

namespace coolgym_webapi.Contexts.BillingInvoices.Interfaces.REST.Transform;

/// <summary>
/// Assembler to transform CreateInvoiceResource to CreateInvoiceCommand
/// </summary>
public static class CreateInvoiceCommandFromResourceAssembler
{
    public static CreateInvoiceCommand ToCommandFromResource(CreateInvoiceResource resource)
    {
        return new CreateInvoiceCommand(
            UserId: resource.UserId,
            CompanyName: resource.CompanyName,
            Amount: resource.Amount,
            Currency: resource.Currency,
            Status: resource.Status,
            IssuedAt: DateTime.Parse(resource.IssuedAt),
            PaidAt: resource.PaidAt != null ? DateTime.Parse(resource.PaidAt) : null
        );
    }
}
