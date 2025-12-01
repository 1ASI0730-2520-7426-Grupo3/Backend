using coolgym_webapi.Contexts.Rentals.Domain.Commands;
using coolgym_webapi.Contexts.Rentals.Interfaces.REST.Resources;

namespace coolgym_webapi.Contexts.Rentals.Interfaces.REST.Transform;

public static class CreateRentalRequestCommandFromResourceAssembler
{
    public static CreateRentalRequestCommand ToCommandFromResource(CreateRentalRequestResource resource)
    {
        return new CreateRentalRequestCommand(
            resource.EquipmentId,
            resource.ClientId,
            resource.MonthlyPrice,
            resource.Notes);
    }
}
