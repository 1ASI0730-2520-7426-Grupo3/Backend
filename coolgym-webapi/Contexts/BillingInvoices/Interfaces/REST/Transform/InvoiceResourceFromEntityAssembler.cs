using coolgym_webapi.Contexts.BillingInvoices.Domain.Model.Entities;
using coolgym_webapi.Contexts.BillingInvoices.Interfaces.REST.Resources;

namespace coolgym_webapi.Contexts.BillingInvoices.Interfaces.REST.Transform;

/// <summary>
///     Assembler to transform BillingInvoice domain entity to InvoiceResource DTO
/// </summary>
public static class InvoiceResourceFromEntityAssembler
{
    public static InvoiceResource ToResourceFromEntity(BillingInvoice entity)
    {
        return new InvoiceResource(
            entity.Id,
            entity.UserId,
            entity.CompanyName,
            entity.Amount.Amount,
            entity.Amount.Currency,
            entity.Status.Value,
            entity.IssuedAt.ToString("yyyy-MM-dd"),
            entity.PaidAt?.ToString("yyyy-MM-dd")
        );
    }
}