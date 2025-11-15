namespace coolgym_webapi.Contexts.RentalCatalog.Domain.Exceptions;

public class RentalItemNotFoundException : Exception
{
    public RentalItemNotFoundException(int id)
        : base($"Rental item with id {id} was not found.") { }
}