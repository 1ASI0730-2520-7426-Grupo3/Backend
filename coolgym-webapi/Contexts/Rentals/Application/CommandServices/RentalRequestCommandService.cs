using coolgym_webapi.Contexts.BillingInvoices.Domain.Commands;
using coolgym_webapi.Contexts.BillingInvoices.Domain.Services;
using coolgym_webapi.Contexts.ClientPlans.Domain.Repositories;
using coolgym_webapi.Contexts.Rentals.Domain.Commands;
using coolgym_webapi.Contexts.Rentals.Domain.Model.Entities;
using coolgym_webapi.Contexts.Rentals.Domain.Repositories;
using coolgym_webapi.Contexts.Rentals.Domain.Services;
using coolgym_webapi.Contexts.Security.Domain.Infrastructure;
using coolgym_webapi.Contexts.Shared.Domain.Repositories;

namespace coolgym_webapi.Contexts.Rentals.Application.CommandServices;

public class RentalRequestCommandService(
    IRentalRequestRepository rentalRequestRepository,
    IInvoiceCommandService invoiceCommandService,
    IUserRepository userRepository,
    IClientPlanRepository clientPlanRepository,
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

        // ===== CLIENT PLAN LIMIT VALIDATION =====
        // Check if client has reached their plan limit before approving
        var client = await userRepository.FindByIdAsync(rentalRequest.ClientId);
        if (client == null)
            throw new InvalidOperationException("Client not found");

        Console.WriteLine($"[VALIDATION] Client ID: {rentalRequest.ClientId}, ClientPlanId: {client.ClientPlanId}");

        if (client.ClientPlanId.HasValue)
        {
            var clientPlan = await clientPlanRepository.FindByIdAsync(client.ClientPlanId.Value);
            if (clientPlan != null)
            {
                Console.WriteLine($"[VALIDATION] Client Plan: {clientPlan.Name}, Max: {clientPlan.MaxEquipmentAccess}");

                // Count client's currently approved rental requests (active machines)
                var clientRequests = await rentalRequestRepository.FindByClientIdAsync(rentalRequest.ClientId);
                var approvedCount = clientRequests.Count(r => r.Status == "approved" && r.IsDeleted == 0);

                Console.WriteLine($"[VALIDATION] Approved count: {approvedCount}, Limit: {clientPlan.MaxEquipmentAccess}");
                Console.WriteLine($"[VALIDATION] All requests statuses: {string.Join(", ", clientRequests.Select(r => $"{r.Id}:{r.Status}:{r.IsDeleted}"))}");

                // Check if client has reached their plan limit
                if (approvedCount >= clientPlan.MaxEquipmentAccess)
                {
                    throw new InvalidOperationException(
                        $"Client has reached their plan limit of {clientPlan.MaxEquipmentAccess} machines " +
                        $"({approvedCount}/{clientPlan.MaxEquipmentAccess}). " +
                        $"They must upgrade their plan before accepting this request.");
                }
            }
            else
            {
                Console.WriteLine($"[VALIDATION] Client plan not found for ID: {client.ClientPlanId.Value}");
            }
        }
        else
        {
            Console.WriteLine($"[VALIDATION] Client has no plan assigned");
        }
        // ===== END VALIDATION =====

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