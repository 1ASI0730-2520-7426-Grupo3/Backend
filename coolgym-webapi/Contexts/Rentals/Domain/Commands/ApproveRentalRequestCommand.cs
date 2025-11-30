namespace coolgym_webapi.Contexts.Rentals.Domain.Commands;

public record ApproveRentalRequestCommand(int RentalRequestId, int ProviderId);
