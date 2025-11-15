using coolgym_webapi.Contexts.RentalCatalog.Domain.Commands;
using coolgym_webapi.Contexts.RentalCatalog.Domain.Model.Entities;
using coolgym_webapi.Contexts.RentalCatalog.Interfaces.REST.Resources;

namespace coolgym_webapi.Contexts.RentalCatalog.Interfaces.REST.Transform;

public static class RentalItemAssemblers
{
    public static RentalItemResource ToResource(RentalItem e) =>
        new(e.Id, e.Name, e.Type, e.Model, e.MonthlyPrice.Amount, e.MonthlyPrice.Currency, e.ImageUrl, e.IsAvailable);

    public static CreateRentalItemCommand ToCommand(this CreateRentalItemResource r) =>
        new(r.EquipmentName, r.Type, r.Model, r.MonthlyPriceUSD, r.Currency, r.ImageUrl, r.IsAvailable);

    public static UpdateRentalItemCommand ToCommand(this UpdateRentalItemResource r) =>
        new(r.Id, r.EquipmentName, r.Type, r.Model, r.MonthlyPriceUSD, r.Currency, r.ImageUrl, r.IsAvailable);
}