using coolgym_webapi.Contexts.maintenance.Domain.Exceptions;
using coolgym_webapi.Contexts.Shared.Domain.Model.Entities;
using InvalidDataException = coolgym_webapi.Contexts.maintenance.Domain.Exceptions.InvalidDataException;

namespace coolgym_webapi.Contexts.maintenance.Domain.Model.Entities;

public class MaintenanceRequest : BaseEntity
{
    private const int MinObservationLength = 10;

    public const string PendingStatus = "pending";
    public const string CompletedStatus = "completed";
    public const string CancelledStatus = "cancelled";
    private static readonly TimeSpan MinimumLeadTime = TimeSpan.FromHours(24);

    protected MaintenanceRequest()
    {
    }

    public MaintenanceRequest(int equipmentId, DateTime selectedDate, string observation, int requestedByUserId, int? assignedToProviderId = null)
    {
        if (equipmentId <= 0)
            throw new InvalidDataException("Equipment identifier must be positive.");

        if (requestedByUserId <= 0)
            throw new InvalidDataException("User identifier must be positive.");

        if (string.IsNullOrWhiteSpace(observation))
            throw InvalidDataException.EmptyObservation();

        if (observation.Length < MinObservationLength)
            throw InvalidDataException.ObservationTooShort(observation.Length, MinObservationLength);

        var now = DateTime.UtcNow;

        if (selectedDate <= now)
            throw InvalidDataException.InvalidSelectedDate();

        var difference = selectedDate - now;
        if (difference < MinimumLeadTime)
            throw InvalidDataException.SelectedDateTooSoon(difference, MinimumLeadTime);

        EquipmentId = equipmentId;
        RequestedByUserId = requestedByUserId;
        AssignedToProviderId = assignedToProviderId;
        SelectedDate = selectedDate;
        Observation = observation.Trim();
        Status = PendingStatus;
        CreatedDate = now;
    }

    public int EquipmentId { get; private set; }
    public int RequestedByUserId { get; private set; }
    public int? AssignedToProviderId { get; private set; }
    public DateTime SelectedDate { get; private set; }
    public string Observation { get; private set; } = string.Empty;
    public string Status { get; private set; } = PendingStatus;

    public void UpdateStatus(string newStatus)
    {
        if (string.IsNullOrWhiteSpace(newStatus))
            throw InvalidDataException.InvalidStatus(newStatus);

        var normalizedStatus = newStatus.ToLowerInvariant();

        if (normalizedStatus != PendingStatus &&
            normalizedStatus != CompletedStatus &&
            normalizedStatus != CancelledStatus)
            throw InvalidDataException.InvalidStatus(newStatus);

        if (Status == CompletedStatus && normalizedStatus != CompletedStatus)
            throw new InvalidMaintenanceRequestStatusException();

        Status = normalizedStatus;
        UpdatedDate = DateTime.UtcNow;
    }

    public void AssignToProvider(int providerId)
    {
        if (providerId <= 0)
            throw new InvalidDataException("Provider identifier must be positive.");

        AssignedToProviderId = providerId;
        UpdatedDate = DateTime.UtcNow;
    }
}