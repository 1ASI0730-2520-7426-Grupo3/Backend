namespace coolgym_webapi.Contexts.Equipments.Domain.Exceptions;

public class EquipmentNotFoundException : Exception
{
    public const string ResourceKey = "EquipmentNotFound";

    public EquipmentNotFoundException(int id)
    {
        EquipmentId = id;
    }

    public int EquipmentId { get; }
}

public class DuplicateSerialNumberException : Exception
{
    public const string ResourceKey = "DuplicateSerialNumber";

    public DuplicateSerialNumberException(string serialNumber)
    {
        SerialNumber = serialNumber;
    }

    public string SerialNumber { get; }
}

public class EquipmentPoweredOnException : Exception
{
    public const string ResourceKey = "EquipmentPoweredOn";

    public EquipmentPoweredOnException(string name)
    {
        EquipmentName = name;
    }

    public string EquipmentName { get; }
}

public class EquipmentInMaintenanceException : Exception
{
    public const string ResourceKey = "EquipmentInMaintenance";

    public EquipmentInMaintenanceException(string name)
    {
        EquipmentName = name;
    }

    public string EquipmentName { get; }
}

public class InvalidStatusException : Exception
{
    public InvalidStatusException(string status)
    {
        AttemptedStatus = status;
    }

    public string AttemptedStatus { get; }
}

public class InvalidLocationException : Exception
{
    private InvalidLocationException(string key) : base(key)
    {
    }

    public static InvalidLocationException EmptyName()
    {
        return new InvalidLocationException("LocationNameEmpty");
    }

    public static InvalidLocationException NameTooLong(int length)
    {
        return new InvalidLocationException($"LocationNameTooLong:{length}");
    }

    public static InvalidLocationException EmptyAddress()
    {
        return new InvalidLocationException("LocationAddressEmpty");
    }

    public static InvalidLocationException AddressTooLong(int length)
    {
        return new InvalidLocationException($"LocationAddressTooLong:{length}");
    }
}

public class InvalidControlSettingsException : Exception
{
    private InvalidControlSettingsException(string key) : base(key)
    {
    }

    public static InvalidControlSettingsException EmptyPower()
    {
        return new InvalidControlSettingsException("ControlPowerEmpty");
    }

    public static InvalidControlSettingsException EmptyStatus()
    {
        return new InvalidControlSettingsException("ControlStatusEmpty");
    }

    public static InvalidControlSettingsException NegativeMinLevel(int minLevel)
    {
        return new InvalidControlSettingsException($"ControlNegativeMinLevel:{minLevel}");
    }

    public static InvalidControlSettingsException InvalidRange(int minLevel, int maxLevel)
    {
        return new InvalidControlSettingsException($"ControlInvalidRange:{minLevel}:{maxLevel}");
    }

    public static InvalidControlSettingsException CurrentLevelOutOfRange(int currentLevel, int minLevel, int maxLevel)
    {
        return new InvalidControlSettingsException(
            $"ControlCurrentLevelOutOfRange:{currentLevel}:{minLevel}:{maxLevel}");
    }

    public static InvalidControlSettingsException SetLevelOutOfRange(int setLevel, int minLevel, int maxLevel)
    {
        return new InvalidControlSettingsException($"ControlSetLevelOutOfRange:{setLevel}:{minLevel}:{maxLevel}");
    }
}

public class InvalidUsageStatsException : Exception
{
    private InvalidUsageStatsException(string key) : base(key)
    {
    }

    public static InvalidUsageStatsException NegativeTotalMinutes(int totalMinutes)
    {
        return new InvalidUsageStatsException($"UsageTotalMinutesNegative:{totalMinutes}");
    }

    public static InvalidUsageStatsException NegativeTodayMinutes(int todayMinutes)
    {
        return new InvalidUsageStatsException($"UsageTodayMinutesNegative:{todayMinutes}");
    }

    public static InvalidUsageStatsException NegativeCalories(int calories)
    {
        return new InvalidUsageStatsException($"UsageNegativeCalories:{calories}");
    }

    public static InvalidUsageStatsException TodayExceedsTotal(int todayMinutes, int totalMinutes)
    {
        return new InvalidUsageStatsException($"UsageTodayExceedsTotal:{todayMinutes}:{totalMinutes}");
    }
}