namespace coolgym_webapi.Contexts.Rentals.Interfaces.REST.Resources;

public record RentalRequestResource(
    int Id,
    int EquipmentId,
    string? EquipmentName,
    string? EquipmentType,
    string? EquipmentImage,
    int ClientId,
    string? ClientEmail,
    int? ProviderId,
    DateTime RequestDate,
    string Status,
    string? Notes,
    decimal MonthlyPrice,
    DateTime CreatedDate,
    DateTime UpdatedDate);