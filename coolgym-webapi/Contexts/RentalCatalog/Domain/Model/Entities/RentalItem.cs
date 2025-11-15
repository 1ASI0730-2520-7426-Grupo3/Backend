using coolgym_webapi.Contexts.RentalCatalog.Domain.Model.ValueObjects;
using coolgym_webapi.Contexts.Shared.Domain.Model.Entities;

namespace coolgym_webapi.Contexts.RentalCatalog.Domain.Model.Entities;
public class RentalItem : BaseEntity
{
    // Inicialización "null-forgiving" para satisfacer el analizador (se asignan en el ctor)
    public string Name { get; private set; } = null!;
    public string Type { get; private set; } = null!;
    public string Model { get; private set; } = null!;
    public Money MonthlyPrice { get; private set; } = null!;
    public string ImageUrl { get; private set; } = string.Empty;
    public bool IsAvailable { get; private set; }

    // EF Core
    private RentalItem() { }

    public RentalItem(string name, string type, string model, Money monthlyPrice,
                      string imageUrl, bool isAvailable = true)
    {
        UpdateBasicInfo(name, type, model, imageUrl);
        UpdateMonthlyPrice(monthlyPrice);
        IsAvailable = isAvailable;
        Touch(); // actualiza UpdatedDate
    }

    public void UpdateBasicInfo(string name, string type, string model, string imageUrl)
    {
        if (string.IsNullOrWhiteSpace(name))  throw new ArgumentException("Name required",  nameof(name));
        if (string.IsNullOrWhiteSpace(type))  throw new ArgumentException("Type required",  nameof(type));
        if (string.IsNullOrWhiteSpace(model)) throw new ArgumentException("Model required", nameof(model));

        Name = name.Trim();
        Type = type.Trim();
        Model = model.Trim();
        ImageUrl = imageUrl?.Trim() ?? string.Empty;
        Touch();
    }

    public void UpdateMonthlyPrice(Money monthlyPrice)
    {
        if (monthlyPrice is null || monthlyPrice.Amount < 0)
            throw new ArgumentException("Monthly price invalid", nameof(monthlyPrice));

        MonthlyPrice = monthlyPrice;
        Touch();
    }

    public void SetAvailability(bool available)
    {
        IsAvailable = available;
        Touch();
    }

    // Como tu BaseEntity no trae Touch(), lo definimos aquí
    private void Touch()
    {
        // Si tu BaseEntity expone UpdatedDate con set; privado, cambia este método por uno vacío.
        UpdatedDate = DateTime.UtcNow;
    }
}