namespace coolgym_webapi.Contexts.RentalCatalog.Domain.Queries;

public record GetAllRentalItemsQuery();
public record GetRentalItemByIdQuery(int Id);
public record GetRentalItemsByTypeQuery(string Type);
public record GetAvailableRentalItemsQuery();