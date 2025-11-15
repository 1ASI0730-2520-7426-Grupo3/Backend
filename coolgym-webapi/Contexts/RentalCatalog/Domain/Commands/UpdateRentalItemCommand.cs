namespace coolgym_webapi.Contexts.RentalCatalog.Domain.Commands;

public record UpdateRentalItemCommand(int Id, string Name, string Type, string Model,
    decimal MonthlyPriceUSD, string Currency, string ImageUrl, bool IsAvailable);