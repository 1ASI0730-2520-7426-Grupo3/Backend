namespace coolgym_webapi.Contexts.Rentals.Domain.Commands;

public record CreateRentalRequestCommand(
    int EquipmentId,
    int ClientId,
    decimal MonthlyPrice,
    string? Notes);