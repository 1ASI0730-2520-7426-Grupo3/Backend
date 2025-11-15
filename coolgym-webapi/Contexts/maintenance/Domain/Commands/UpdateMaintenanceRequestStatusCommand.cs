namespace coolgym_webapi.Contexts.maintenance.Domain.Commands;

public record UpdateMaintenanceRequestStatusCommand(int Id, string Status);