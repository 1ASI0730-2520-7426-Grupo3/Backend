using coolgym_webapi.Contexts.Rentals.Domain.Model.Entities;
using coolgym_webapi.Contexts.Shared.Domain.Repositories;

namespace coolgym_webapi.Contexts.Rentals.Domain.Repositories;

public interface IRentalRequestRepository : IBaseRepository<RentalRequest>
{
    Task<IEnumerable<RentalRequest>> FindByClientIdAsync(int clientId);
    Task<IEnumerable<RentalRequest>> FindByStatusAsync(string status);
}