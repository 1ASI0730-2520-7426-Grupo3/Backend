using coolgym_webapi.Contexts.RentalCatalog.Domain.Model.Entities;
using coolgym_webapi.Contexts.RentalCatalog.Domain.Queries;
using coolgym_webapi.Contexts.RentalCatalog.Domain.Repositories;

namespace coolgym_webapi.Contexts.RentalCatalog.Application.QueryServices;

public interface IRentalCatalogQueryService
{
    Task<IEnumerable<RentalItem>> Handle(GetAllRentalItemsQuery q);
    Task<RentalItem?> Handle(GetRentalItemByIdQuery q);
    Task<IEnumerable<RentalItem>> Handle(GetRentalItemsByTypeQuery q);
    Task<IEnumerable<RentalItem>> Handle(GetAvailableRentalItemsQuery q);
}

public class RentalCatalogQueryService : IRentalCatalogQueryService
{
    private readonly IRentalItemRepository _repo;
    public RentalCatalogQueryService(IRentalItemRepository repo) { _repo = repo; }

    public Task<IEnumerable<RentalItem>> Handle(GetAllRentalItemsQuery q) => _repo.ListAsync();
    public Task<RentalItem?> Handle(GetRentalItemByIdQuery q) => _repo.FindByIdAsync(q.Id);
    public Task<IEnumerable<RentalItem>> Handle(GetRentalItemsByTypeQuery q) => _repo.FindByTypeAsync(q.Type);
    public Task<IEnumerable<RentalItem>> Handle(GetAvailableRentalItemsQuery q) => _repo.FindAvailableAsync();
}