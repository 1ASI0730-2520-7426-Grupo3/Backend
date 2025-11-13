using coolgym_webapi.Contexts.maintenance.Domain.Commands;
using coolgym_webapi.Contexts.maintenance.Domain.Model.Entities;

namespace coolgym_webapi.Contexts.maintenance.Domain.Services;

public interface IMaintenanceRequestCommandService
{
    Task<MaintenanceRequest> Handle(CreateMaintenanceRequestCommand command);
    
    Task<MaintenanceRequest?> Handle(UpdateMaintenanceRequestStatusCommand command);
    
    Task<bool> Handle(DeleteMaintenanceRequestCommand command);
}