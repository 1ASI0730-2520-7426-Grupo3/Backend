using coolgym_webapi.Contexts.BillingInvoices.Domain.Model.Entities;
using coolgym_webapi.Contexts.BillingInvoices.Interfaces.REST.Resources;

namespace coolgym_webapi.Contexts.BillingInvoices.Interfaces.REST.Transform;

/// <summary>
/// Assembler to transform BillingInvoice domain entity to InvoiceResource DTO
/// </summary>
public static class InvoiceResourceFromEntityAssembler
{
    public static InvoiceResource ToResourceFromEntity(BillingInvoice entity)
    {
        return new InvoiceResource(
            Id: entity.Id,
            UserId: entity.UserId,
            CompanyName: entity.CompanyName,
            Amount: entity.Amount.Amount,
            Currency: entity.Amount.Currency,
            Status: entity.Status.Value,
            IssuedAt: entity.IssuedAt.ToString("yyyy-MM-dd"),
            PaidAt: entity.PaidAt?.ToString("yyyy-MM-dd")
        );
    }
}
