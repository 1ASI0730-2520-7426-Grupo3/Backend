using coolgym_webapi.Contexts.Equipments.Domain.Exceptions;
using coolgym_webapi.Contexts.Equipments.Domain.Queries;
using coolgym_webapi.Contexts.Equipments.Domain.Services;
using coolgym_webapi.Contexts.maintenance.Domain.Commands;
using coolgym_webapi.Contexts.maintenance.Domain.Exceptions;
using coolgym_webapi.Contexts.maintenance.Domain.Model.Entities;
using coolgym_webapi.Contexts.maintenance.Domain.Repositories;
using coolgym_webapi.Contexts.maintenance.Domain.Services;
using coolgym_webapi.Contexts.Shared.Domain.Repositories;

namespace coolgym_webapi.Contexts.maintenance.Application.CommandServices;

public class MaintenanceRequestCommandService(
    IMaintenanceRequestRepository maintenanceRequestRepository,
    IUnitOfWork unitOfWork,
    IEquipmentQueryService equipmentQueryService) : IMaintenanceRequestCommandService
{
    private const string PendingStatus = "pending";

    public async Task<MaintenanceRequest> Handle(CreateMaintenanceRequestCommand command)
    {
        var existingMaintenanceRequest =
            await maintenanceRequestRepository.FindByEquipmentIdAsync(command.EquipmentId);

        if (existingMaintenanceRequest is not null &&
            string.Equals(existingMaintenanceRequest.Status, PendingStatus, StringComparison.OrdinalIgnoreCase))
            throw new DuplicateEquipmentMaintenanceRequestException(command.EquipmentId);

        var equipment = await equipmentQueryService.Handle(new GetEquipmentById(command.EquipmentId));
        if (equipment is null) throw new EquipmentNotFoundException(command.EquipmentId);

        var maintenanceRequest = new MaintenanceRequest(
            command.EquipmentId,
            command.SelectedDate,
            command.Observation,
            command.RequestedByUserId,
            command.AssignedToProviderId);

        await maintenanceRequestRepository.AddAsync(maintenanceRequest);
        await unitOfWork.CompleteAsync();

        return maintenanceRequest;
    }

    public async Task<MaintenanceRequest?> Handle(UpdateMaintenanceRequestStatusCommand command)
    {
        var maintenanceRequest = await maintenanceRequestRepository.FindByIdAsync(command.Id);
        if (maintenanceRequest == null) throw new MaintenanceRequestNotFoundException(command.Id);

        if (string.Equals(command.Status, PendingStatus, StringComparison.OrdinalIgnoreCase) &&
            string.Equals(maintenanceRequest.Status, PendingStatus, StringComparison.OrdinalIgnoreCase))
            throw new MaintenanceRequestIsAlreadyPendingException();

        if (string.Equals(command.Status, "completed", StringComparison.OrdinalIgnoreCase))
        {
            maintenanceRequest.UpdateStatus(command.Status);
            maintenanceRequestRepository.Update(maintenanceRequest);
            await unitOfWork.CompleteAsync();
            return maintenanceRequest;
        }

        if (string.Equals(command.Status, PendingStatus, StringComparison.OrdinalIgnoreCase))
            throw new MaintenanceRequestIsAlreadyPendingException();

        throw new InvalidMaintenanceRequestStatusException();
    }

    public async Task<MaintenanceRequest?> Handle(AssignMaintenanceRequestCommand command)
    {
        var maintenanceRequest = await maintenanceRequestRepository.FindByIdAsync(command.Id);
        if (maintenanceRequest == null) throw new MaintenanceRequestNotFoundException(command.Id);

        maintenanceRequest.AssignToProvider(command.ProviderId);
        maintenanceRequestRepository.Update(maintenanceRequest);
        await unitOfWork.CompleteAsync();

        return maintenanceRequest;
    }

    public async Task<bool> Handle(DeleteMaintenanceRequestCommand command)
    {
        var maintenanceRequest = await maintenanceRequestRepository.FindByIdAsync(command.Id);
        if (maintenanceRequest == null) throw new MaintenanceRequestNotFoundException(command.Id);

        if (string.Equals(maintenanceRequest.Status, PendingStatus, StringComparison.OrdinalIgnoreCase))
            throw new ArgumentException("Cannot delete a pending maintenance request.");

        maintenanceRequestRepository.Remove(maintenanceRequest);
        await unitOfWork.CompleteAsync();

        return true;
    }
}