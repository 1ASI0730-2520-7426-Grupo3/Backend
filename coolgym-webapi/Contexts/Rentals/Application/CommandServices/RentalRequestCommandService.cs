using coolgym_webapi.Contexts.BillingInvoices.Domain.Commands;
using coolgym_webapi.Contexts.BillingInvoices.Domain.Services;
using coolgym_webapi.Contexts.Rentals.Domain.Commands;
using coolgym_webapi.Contexts.Rentals.Domain.Model.Entities;
using coolgym_webapi.Contexts.Rentals.Domain.Repositories;
using coolgym_webapi.Contexts.Rentals.Domain.Services;
using coolgym_webapi.Contexts.Shared.Domain.Repositories;

namespace coolgym_webapi.Contexts.Rentals.Application.CommandServices;

public class RentalRequestCommandService(
    IRentalRequestRepository rentalRequestRepository,
    IInvoiceCommandService invoiceCommandService,
    IUnitOfWork unitOfWork) : IRentalRequestCommandService
{
    public async Task<RentalRequest> Handle(CreateRentalRequestCommand command)
    {
        // Check if a pending rental request already exists for this equipment by this client
        var existingRequests = await rentalRequestRepository.FindByClientIdAsync(command.ClientId);
        var hasPendingRequest = existingRequests.Any(r =>
            r.EquipmentId == command.EquipmentId &&
            r.Status == "pending");

        if (hasPendingRequest)
            throw new InvalidOperationException("A pending rental request already exists for this equipment");

        var rentalRequest = new RentalRequest(
            command.EquipmentId,
            command.ClientId,
            command.MonthlyPrice,
            command.Notes
        );

        await rentalRequestRepository.AddAsync(rentalRequest);
        await unitOfWork.CompleteAsync();

        // Reload with navigation properties (Equipment, Client, Provider)
        return await rentalRequestRepository.FindByIdAsync(rentalRequest.Id) ?? rentalRequest;
    }

    public async Task<RentalRequest?> Handle(UpdateRentalRequestStatusCommand command)
    {
        var rentalRequest = await rentalRequestRepository.FindByIdAsync(command.Id);
        if (rentalRequest == null) return null;

        rentalRequest.UpdateStatus(command.Status);
        rentalRequestRepository.Update(rentalRequest);
        await unitOfWork.CompleteAsync();

        return rentalRequest;
    }

    public async Task<RentalRequest?> Handle(ApproveRentalRequestCommand command)
    {
        var rentalRequest = await rentalRequestRepository.FindByIdAsync(command.RentalRequestId);
        if (rentalRequest == null) return null;

        // Approve the rental request
        rentalRequest.Approve(command.ProviderId);
        rentalRequestRepository.Update(rentalRequest);

        // Auto-create invoice for the client
        var clientName = rentalRequest.Client?.Email ?? $"Client #{rentalRequest.ClientId}";
        var invoiceCommand = new CreateInvoiceCommand(
            rentalRequest.ClientId,
            clientName,
            rentalRequest.MonthlyPrice,
            "USD",
            "pending",
            DateTime.UtcNow,
            null,
            command.ProviderId,
            null,
            rentalRequest.Id
        );

        await invoiceCommandService.Handle(invoiceCommand);
        await unitOfWork.CompleteAsync();

        return rentalRequest;
    }
}