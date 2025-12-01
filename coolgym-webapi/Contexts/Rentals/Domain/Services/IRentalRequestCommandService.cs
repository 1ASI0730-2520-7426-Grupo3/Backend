using coolgym_webapi.Contexts.Rentals.Domain.Commands;
using coolgym_webapi.Contexts.Rentals.Domain.Model.Entities;

namespace coolgym_webapi.Contexts.Rentals.Domain.Services;

public interface IRentalRequestCommandService
{
    Task<RentalRequest> Handle(CreateRentalRequestCommand command);
    Task<RentalRequest?> Handle(UpdateRentalRequestStatusCommand command);
    Task<RentalRequest?> Handle(ApproveRentalRequestCommand command);
}
