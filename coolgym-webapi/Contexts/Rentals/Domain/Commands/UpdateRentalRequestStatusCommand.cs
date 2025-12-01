namespace coolgym_webapi.Contexts.Rentals.Domain.Commands;

public record UpdateRentalRequestStatusCommand(int Id, string Status);