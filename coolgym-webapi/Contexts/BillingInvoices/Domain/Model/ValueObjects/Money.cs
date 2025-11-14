namespace coolgym_webapi.Contexts.BillingInvoices.Domain.Model.ValueObjects;

/// <summary>
/// Represents a monetary amount with currency
/// Ensures amount is always positive and currency is valid
/// </summary>
public record Money
{
    public decimal Amount { get; init; }
    public string Currency { get; init; }

    private Money(decimal amount, string currency)
    {
        Amount = amount;
        Currency = currency;
    }

    public static Money? Create(decimal amount, string currency)
    {
        if (amount < 0) return null;
        if (string.IsNullOrWhiteSpace(currency)) return null;

        // Validate currency code (ISO 4217 format - 3 letters)
        if (currency.Length != 3) return null;

        return new Money(amount, currency.ToUpper());
    }

    public static Money operator +(Money a, Money b)
    {
        if (a.Currency != b.Currency)
            throw new InvalidOperationException("Cannot add money with different currencies");

        return new Money(a.Amount + b.Amount, a.Currency);
    }
}
