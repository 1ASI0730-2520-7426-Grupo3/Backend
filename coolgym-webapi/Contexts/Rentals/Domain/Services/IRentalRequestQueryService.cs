using coolgym_webapi.Contexts.Rentals.Domain.Model.Entities;
using coolgym_webapi.Contexts.Rentals.Domain.Queries;

namespace coolgym_webapi.Contexts.Rentals.Domain.Services;

public interface IRentalRequestQueryService
{
    Task<IEnumerable<RentalRequest>> Handle(GetAllRentalRequests query);
    Task<RentalRequest?> Handle(GetRentalRequestById query);
    Task<IEnumerable<RentalRequest>> Handle(GetRentalRequestsByClientId query);
}
