using coolgym_webapi.Contexts.maintenance.Domain.Commands;
using coolgym_webapi.Contexts.maintenance.Domain.Exceptions;
using coolgym_webapi.Contexts.maintenance.Domain.Model.Entities;
using coolgym_webapi.Contexts.maintenance.Domain.Repositories;
using coolgym_webapi.Contexts.maintenance.Domain.Services;
using coolgym_webapi.Contexts.Shared.Domain.Repositories;

namespace coolgym_webapi.Contexts.maintenance.Application.CommandServices;

public class MaintenanceRequestCommandService(IMaintenanceRequestRepository maintenanceRequestRepository, IUnitOfWork unitOfWork) 
    : IMaintenanceRequestCommandService
{
    public async Task<MaintenanceRequest> Handle(CreateMaintenanceRequestCommand command)
    {
        var existingMaintenanceRequest = await maintenanceRequestRepository
            .FindByEquipmentIdAsync(command.EquipmentId);
        if (existingMaintenanceRequest != null) 
        {
            throw new InvalidOperationException("A maintenance request for this equipment already exists.");
        }
        
        var maintenanceRequest = new MaintenanceRequest(
            command.EquipmentId,
            command.SelectedDate,
            command.Observation
        );
        
        await maintenanceRequestRepository.AddAsync(maintenanceRequest);
        await unitOfWork.CompleteAsync();
        return maintenanceRequest;
    }
    
    public async Task<MaintenanceRequest?> Handle(UpdateMaintenanceRequestStatusCommand command)
    {
        var maintenanceRequest = await maintenanceRequestRepository.FindByIdAsync(command.Id);
        if (maintenanceRequest == null)
            throw new MaintenanceRequestNotFoundException(command.Id);

        if (command.Status == "accepted")
        {
            maintenanceRequest.UpdateStatus(command.Status);

            maintenanceRequestRepository.Update(maintenanceRequest);
            await unitOfWork.CompleteAsync();

            return maintenanceRequest;
        }else if (command.Status == "pending")
        {
            throw new MaintenanceRequestIsAlreadyPendingException();
        }
        else
        {
            throw new InvalidMaintenanceRequestStatusException();
        }
            
        

    }
    
    public async Task<bool> Handle(DeleteMaintenanceRequestCommand command)
    {
        var maintenanceRequest = await maintenanceRequestRepository.FindByIdAsync(command.Id);

        if (maintenanceRequest == null)
            throw new MaintenanceRequestNotFoundException(command.Id);

        
        if (maintenanceRequest.Status == "pending")
            throw new ArgumentException();

        
        maintenanceRequestRepository.Update(maintenanceRequest);
        await unitOfWork.CompleteAsync();

        return true;
    }
}

