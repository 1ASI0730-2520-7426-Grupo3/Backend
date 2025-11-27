using coolgym_webapi.Contexts.Shared.Domain.Model.Entities;
using coolgym_webapi.Contexts.maintenance.Domain.Exceptions;
using InvalidDataException = coolgym_webapi.Contexts.maintenance.Domain.Exceptions.InvalidDataException;

namespace coolgym_webapi.Contexts.maintenance.Domain.Model.Entities;

public class MaintenanceRequest : BaseEntity
{
    private const int MinObservationLength = 10;
    private static readonly TimeSpan MinimumLeadTime = TimeSpan.FromHours(24);

    public const string PendingStatus = "pending";
    public const string CompletedStatus = "completed";
    public const string CancelledStatus = "cancelled";

    protected MaintenanceRequest()
    {
    }

    public int EquipmentId { get; private set; }
    public DateTime SelectedDate { get; private set; }
    public string Observation { get; private set; } = string.Empty;
    public string Status { get; private set; } = PendingStatus;

    public MaintenanceRequest(int equipmentId, DateTime selectedDate, string observation)
    {
        if (equipmentId <= 0)
            throw new InvalidDataException("Equipment identifier must be positive.");

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
        SelectedDate = selectedDate;
        Observation = observation.Trim();
        Status = PendingStatus;
        CreatedDate = now;
    }

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
}
