namespace coolgym_webapi.Contexts.Equipments.Domain.Exceptions;

public class EquipmentNotFoundException : Exception
{
    public EquipmentNotFoundException(int id)
        : base($"Equipment with id '{id}' not found.")
    {
        EquipmentId = id;
    }

    public int EquipmentId { get; }
}

public class DuplicateSerialNumberException : Exception
{
    public DuplicateSerialNumberException(string serialNumber)
        : base($"Equipment with serial number '{serialNumber}' already exists.")
    {
        SerialNumber = serialNumber;
    }

    public string SerialNumber { get; }
}

public class EquipmentPoweredOnException : Exception
{
    public EquipmentPoweredOnException(string name)
        : base($"Cannot delete equipment '{name}' because it is powered on. Please turn it off first.")
    {
        EquipmentName = name;
    }

    public string EquipmentName { get; }
}

public class EquipmentInMaintenanceException : Exception
{
    public EquipmentInMaintenanceException(string name)
        : base($"Cannot delete equipment '{name}' because it is under maintenance.")
    {
        EquipmentName = name;
    }

    public string EquipmentName { get; }
}

public class InvalidStatusException : Exception
{
    public InvalidStatusException(string status)
        : base($"The status '{status}' is invalid. Status cannot be empty or whitespace.")
    {
        AttemptedStatus = status;
    }

    public string AttemptedStatus { get; }
}

public class InvalidLocationException : Exception
{
    public InvalidLocationException(string message)
        : base($"Invalid location: {message}")
    {
    }

    public static InvalidLocationException EmptyName()
    {
        return new InvalidLocationException("Location name cannot be empty.");
    }

    public static InvalidLocationException NameTooLong(int length)
    {
        return new InvalidLocationException($"Location name cannot exceed 100 characters. Current length: {length}");
    }

    public static InvalidLocationException EmptyAddress()
    {
        return new InvalidLocationException("Location address cannot be empty.");
    }

    public static InvalidLocationException AddressTooLong(int length)
    {
        return new InvalidLocationException($"Location address cannot exceed 200 characters. Current length: {length}");
    }
}

public class InvalidControlSettingsException : Exception
{
    public InvalidControlSettingsException(string message)
        : base($"Invalid control settings: {message}")
    {
    }

    public static InvalidControlSettingsException EmptyPower()
    {
        return new InvalidControlSettingsException("Power status cannot be empty.");
    }

    public static InvalidControlSettingsException EmptyStatus()
    {
        return new InvalidControlSettingsException("Control status cannot be empty.");
    }

    public static InvalidControlSettingsException NegativeMinLevel(int minLevel)
    {
        return new InvalidControlSettingsException($"Minimum level cannot be negative. Value: {minLevel}");
    }

    public static InvalidControlSettingsException InvalidRange(int minLevel, int maxLevel)
    {
        return new InvalidControlSettingsException(
            $"Maximum level ({maxLevel}) must be greater than minimum level ({minLevel}).");
    }

    public static InvalidControlSettingsException CurrentLevelOutOfRange(int currentLevel, int minLevel, int maxLevel)
    {
        return new InvalidControlSettingsException(
            $"Current level ({currentLevel}) must be between {minLevel} and {maxLevel}.");
    }

    public static InvalidControlSettingsException SetLevelOutOfRange(int setLevel, int minLevel, int maxLevel)
    {
        return new InvalidControlSettingsException(
            $"Set level ({setLevel}) must be between {minLevel} and {maxLevel}.");
    }
}

public class InvalidUsageStatsException : Exception
{
    public InvalidUsageStatsException(string message)
        : base($"Invalid usage stats: {message}")
    {
    }

    public static InvalidUsageStatsException NegativeTotalMinutes(int totalMinutes)
    {
        return new InvalidUsageStatsException($"Total minutes cannot be negative. Value: {totalMinutes}");
    }

    public static InvalidUsageStatsException NegativeTodayMinutes(int todayMinutes)
    {
        return new InvalidUsageStatsException($"Today minutes cannot be negative. Value: {todayMinutes}");
    }

    public static InvalidUsageStatsException NegativeCalories(int calories)
    {
        return new InvalidUsageStatsException($"Calories cannot be negative. Value: {calories}");
    }

    public static InvalidUsageStatsException TodayExceedsTotal(int todayMinutes, int totalMinutes)
    {
        return new InvalidUsageStatsException(
            $"Today minutes ({todayMinutes}) cannot exceed total minutes ({totalMinutes}).");
    }
}