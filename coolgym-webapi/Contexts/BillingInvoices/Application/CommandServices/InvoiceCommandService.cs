using coolgym_webapi.Contexts.BillingInvoices.Domain.Commands;
using coolgym_webapi.Contexts.BillingInvoices.Domain.Exceptions;
using coolgym_webapi.Contexts.BillingInvoices.Domain.Model.Entities;
using coolgym_webapi.Contexts.BillingInvoices.Domain.Repositories;
using coolgym_webapi.Contexts.BillingInvoices.Domain.Services;
using coolgym_webapi.Contexts.Shared.Domain.Repositories;

namespace coolgym_webapi.Contexts.BillingInvoices.Application.CommandServices;

/// <summary>
///     Application service that handles billing invoice commands.
/// </summary>
public class InvoiceCommandService(
    IBillingInvoiceRepository invoiceRepository,
    IUnitOfWork unitOfWork) : IInvoiceCommandService
{
    public async Task<BillingInvoice> Handle(CreateInvoiceCommand command)
    {
        var invoice = new BillingInvoice(
            command.UserId,
            command.CompanyName,
            command.Amount,
            command.Currency,
            command.Status,
            command.IssuedAt,
            command.PaidAt
        )
        {
            ProviderId = command.ProviderId,
            MaintenanceRequestId = command.MaintenanceRequestId,
            RentalRequestId = command.RentalRequestId
        };

        await invoiceRepository.AddAsync(invoice);
        await unitOfWork.CompleteAsync();

        return invoice;
    }

    public async Task<BillingInvoice> Handle(MarkInvoiceAsPaidCommand command)
    {
        var invoice = await invoiceRepository.FindByIdAsync(command.InvoiceId);
        if (invoice == null)
            throw new InvoiceNotFoundException(command.InvoiceId);

        invoice.MarkAsPaid(command.PaidAt);

        invoiceRepository.Update(invoice);
        await unitOfWork.CompleteAsync();

        return invoice;
    }

    public async Task<bool> Handle(DeleteInvoiceCommand command)
    {
        var invoice = await invoiceRepository.FindByIdAsync(command.InvoiceId);
        if (invoice == null)
            return false;

        invoiceRepository.Remove(invoice);
        await unitOfWork.CompleteAsync();

        return true;
    }
}