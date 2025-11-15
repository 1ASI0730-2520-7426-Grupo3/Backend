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
    public InvalidDataException(string message)
        : base($"Invalid data: {message}")
    {
    }

    public InvalidDataException EmptyObservation()
    {
        return new InvalidDataException("Observation can't be empty.");
    }

    public static InvalidDataException ObservationTooShort(int length)
    {
        return new InvalidDataException($"Observation can't be too short. Current length: {length}");
    }

    public InvalidDataException VerySoonSelectedDate()
    {
        return new InvalidDataException("Selected date must be almost 1 day in the future.");
    }

    public InvalidDataException InvalidSelectedDate()
    {
        return new InvalidDataException("Selected date must be in the future.");
    }

    public InvalidDataException InvalidStatus()
    {
        return new InvalidDataException("Invalid status.");
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