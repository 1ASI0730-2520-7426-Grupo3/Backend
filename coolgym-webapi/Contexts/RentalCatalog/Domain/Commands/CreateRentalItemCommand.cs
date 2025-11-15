using coolgym_webapi.Contexts.RentalCatalog.Domain.Model.ValueObjects;

namespace coolgym_webapi.Contexts.RentalCatalog.Domain.Commands;

public record CreateRentalItemCommand(
    string Name, string Type, string Model,
    decimal MonthlyPriceUSD, string Currency, string ImageUrl, bool IsAvailable);