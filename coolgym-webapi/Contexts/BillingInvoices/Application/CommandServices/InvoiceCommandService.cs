using coolgym_webapi.Contexts.BillingInvoices.Domain.Commands;
using coolgym_webapi.Contexts.BillingInvoices.Domain.Model.Entities;
using coolgym_webapi.Contexts.BillingInvoices.Domain.Repositories;
using coolgym_webapi.Contexts.BillingInvoices.Domain.Services;
using coolgym_webapi.Contexts.Shared.Domain.Repositories;

namespace coolgym_webapi.Contexts.BillingInvoices.Application.CommandServices;

/// <summary>
///     Application service that handles billing invoice commands
/// </summary>
public class InvoiceCommandService(
    IBillingInvoiceRepository invoiceRepository,
    IUnitOfWork unitOfWork) : IInvoiceCommandService
{
    public async Task<BillingInvoice> Handle(CreateInvoiceCommand command)
    {
        // Create invoice using domain entity constructor (includes validation)
        var invoice = new BillingInvoice(
            command.UserId,
            command.CompanyName,
            command.Amount,
            command.Currency,
            command.Status,
            command.IssuedAt,
            command.PaidAt
        );

        // Persist to repository
        await invoiceRepository.AddAsync(invoice);
        await unitOfWork.CompleteAsync();

        return invoice;
    }

    public async Task<BillingInvoice> Handle(MarkInvoiceAsPaidCommand command)
    {
        // Find invoice
        var invoice = await invoiceRepository.FindByIdAsync(command.InvoiceId);

        if (invoice == null)
            throw new InvalidOperationException($"Invoice with ID {command.InvoiceId} not found");

        // Use domain method (includes business rule validation)
        invoice.MarkAsPaid(command.PaidAt);

        // Update and persist
        invoiceRepository.Update(invoice);
        await unitOfWork.CompleteAsync();

        return invoice;
    }

    public async Task<bool> Handle(DeleteInvoiceCommand command)
    {
        // Find invoice
        var invoice = await invoiceRepository.FindByIdAsync(command.InvoiceId);

        if (invoice == null)
            return false;

        // Soft delete
        invoiceRepository.Remove(invoice);
        await unitOfWork.CompleteAsync();

        return true;
    }
}