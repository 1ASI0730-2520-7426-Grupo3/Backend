using coolgym_webapi.Contexts.RentalCatalog.Domain.Model.Entities;
using coolgym_webapi.Contexts.Shared.Domain.Repositories;

namespace coolgym_webapi.Contexts.RentalCatalog.Domain.Repositories;

public interface IRentalItemRepository : IBaseRepository<RentalItem>
{
    Task<IEnumerable<RentalItem>> FindByTypeAsync(string type);
    Task<IEnumerable<RentalItem>> FindAvailableAsync();
    Task<bool> ExistsByNameAndModelAsync(string name, string model);
}