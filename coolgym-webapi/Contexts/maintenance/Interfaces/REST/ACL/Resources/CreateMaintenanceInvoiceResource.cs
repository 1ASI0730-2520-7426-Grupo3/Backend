namespace coolgym_webapi.Contexts.maintenance.Interfaces.REST.ACL.Resources;

public record CreateMaintenanceInvoiceResource(
    int MaintenanceRequestId,
    int UserId,
    string CompanyName,
    decimal Amount,
    string Currency
);