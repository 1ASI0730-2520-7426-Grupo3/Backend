namespace coolgym_webapi.Contexts.maintenance.Domain.Exceptions;

public class MaintenanceRequestNotFoundException : Exception
{
    public MaintenanceRequestNotFoundException(int id) : base($"Maintenance Request with id '{id}' not found.")
    {
        MaintenanceRequestId = id;
    }

    public int MaintenanceRequestId { get; }
}

public class DuplicateEquipmentMaintenanceRequestException : Exception
{
    public DuplicateEquipmentMaintenanceRequestException(int id) : base(
        "Maintenance request of this equipment already exists.")
    {
        EquipmentId = id;
    }

    public int EquipmentId { get; }
}

public class InvalidDataException : Exception
{
    public InvalidDataException(string message) : base($"Invalid data: {message}")
    {
    }

    public static InvalidDataException EmptyObservation()
    {
        return new InvalidDataException("Observation cannot be empty.");
    }

    public static InvalidDataException ObservationTooShort(int length, int minLength)
    {
        return new InvalidDataException(
            $"Observation must be at least {minLength} characters long. Current length: {length}.");
    }

    public static InvalidDataException InvalidSelectedDate()
    {
        return new InvalidDataException("Selected date must be in the future.");
    }

    public static InvalidDataException SelectedDateTooSoon(TimeSpan difference, TimeSpan minimumDifference)
    {
        return new InvalidDataException(
            $"Selected date must be at least {minimumDifference.TotalHours} hours in the future. Current difference: {difference.TotalHours:F1} hours.");
    }

    public static InvalidDataException InvalidStatus(string status)
    {
        return new InvalidDataException($"Status '{status}' is invalid.");
    }
}

public class InvalidMaintenanceRequestStatusException : Exception
{
    public InvalidMaintenanceRequestStatusException() : base("Invalid Maintenance Request Status.")
    {
    }
}

public class MaintenanceRequestIsAlreadyPendingException : Exception
{
    public MaintenanceRequestIsAlreadyPendingException() : base("Maintenance Request Is Already Pending.")
    {
    }
}