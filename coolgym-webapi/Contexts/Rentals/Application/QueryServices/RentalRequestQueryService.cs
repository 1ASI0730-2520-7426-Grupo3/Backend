using coolgym_webapi.Contexts.Rentals.Domain.Model.Entities;
using coolgym_webapi.Contexts.Rentals.Domain.Queries;
using coolgym_webapi.Contexts.Rentals.Domain.Repositories;
using coolgym_webapi.Contexts.Rentals.Domain.Services;

namespace coolgym_webapi.Contexts.Rentals.Application.QueryServices;

public class RentalRequestQueryService(IRentalRequestRepository rentalRequestRepository)
    : IRentalRequestQueryService
{
    public async Task<IEnumerable<RentalRequest>> Handle(GetAllRentalRequests query)
    {
        return await rentalRequestRepository.ListAsync();
    }

    public async Task<RentalRequest?> Handle(GetRentalRequestById query)
    {
        return await rentalRequestRepository.FindByIdAsync(query.Id);
    }

    public async Task<IEnumerable<RentalRequest>> Handle(GetRentalRequestsByClientId query)
    {
        return await rentalRequestRepository.FindByClientIdAsync(query.ClientId);
    }
}