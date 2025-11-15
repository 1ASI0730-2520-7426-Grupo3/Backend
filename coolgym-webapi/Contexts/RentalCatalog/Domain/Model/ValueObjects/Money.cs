namespace coolgym_webapi.Contexts.RentalCatalog.Domain.Model.ValueObjects;

public record Money(decimal Amount, string Currency)
{
    public static Money Usd(decimal amount) => new(amount, "USD");
};