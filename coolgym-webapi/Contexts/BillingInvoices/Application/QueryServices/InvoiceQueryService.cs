using coolgym_webapi.Contexts.BillingInvoices.Domain.Model.Entities;
using coolgym_webapi.Contexts.BillingInvoices.Domain.Queries;
using coolgym_webapi.Contexts.BillingInvoices.Domain.Repositories;
using coolgym_webapi.Contexts.BillingInvoices.Domain.Services;

namespace coolgym_webapi.Contexts.BillingInvoices.Application.QueryServices;

/// <summary>
/// Application service that handles billing invoice queries
/// </summary>
public class InvoiceQueryService(IBillingInvoiceRepository invoiceRepository) : IInvoiceQueryService
{
    public async Task<IEnumerable<BillingInvoice>> Handle(GetInvoicesByUserId query)
    {
        return await invoiceRepository.FindByUserIdAsync(query.UserId);
    }

    public async Task<BillingInvoice?> Handle(GetInvoiceById query)
    {
        return await invoiceRepository.FindByIdAsync(query.InvoiceId);
    }

    public async Task<IEnumerable<BillingInvoice>> Handle(GetAllInvoices query)
    {
        return await invoiceRepository.ListAsync();
    }
}
