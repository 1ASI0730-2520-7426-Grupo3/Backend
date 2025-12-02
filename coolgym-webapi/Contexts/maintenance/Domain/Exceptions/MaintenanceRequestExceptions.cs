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
    public string ErrorCode { get; }
    public object?[] Parameters { get; }

    public InvalidDataException(string errorCode, params object?[] parameters)
        : base($"Invalid data: {errorCode}")
    {
        ErrorCode = errorCode;
        Parameters = parameters;
    }

    public static InvalidDataException EmptyObservation()
    {
        return new InvalidDataException("ObservationEmpty");
    }

    public static InvalidDataException ObservationTooShort(int length, int minLength)
    {
        return new InvalidDataException("ObservationTooShort", minLength, length);
    }

    public static InvalidDataException InvalidSelectedDate()
    {
        return new InvalidDataException("SelectedDateNotFuture");
    }

    public static InvalidDataException SelectedDateTooSoon(TimeSpan difference, TimeSpan minimumDifference)
    {
        return new InvalidDataException("SelectedDateTooSoon", minimumDifference.TotalHours, difference.TotalHours);
    }

    public static InvalidDataException InvalidStatus(string status)
    {
        return new InvalidDataException("InvalidStatus", status);
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