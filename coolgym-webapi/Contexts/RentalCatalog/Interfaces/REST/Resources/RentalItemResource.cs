namespace coolgym_webapi.Contexts.RentalCatalog.Interfaces.REST.Resources;

public record RentalItemResource(
    int Id,
    string EquipmentName,
    string Type,
    string Model,
    decimal MonthlyPriceUSD,
    string Currency,
    string ImageUrl,
    bool IsAvailable);
    
public record CreateRentalItemResource(
    string EquipmentName,      // "Treadmill Pro X"
    string Type,               // "treadmill"
    string Model,              // "TRX-300"
    decimal MonthlyPriceUSD,   // 200
    string Currency,           // "USD"
    string ImageUrl,           // "/treadmill-pro-x.png"
    bool IsAvailable           // true
);

public record UpdateRentalItemResource(
    int Id,
    string EquipmentName,
    string Type,
    string Model,
    decimal MonthlyPriceUSD,
    string Currency,
    string ImageUrl,
    bool IsAvailable
);