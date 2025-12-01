namespace coolgym_webapi.Contexts.Rentals.Interfaces.REST.Resources;

public record CreateRentalRequestResource(
    int EquipmentId,
    int ClientId,
    decimal MonthlyPrice,
    string? Notes);
