namespace coolgym_webapi.Contexts.Equipments.Domain.Exceptions;

/// <summary>
///     Excepción lanzada cuando no se encuentra un equipo por su ID
///     Usado en: CommandService.Handle(UpdateEquipmentCommand), CommandService.Handle(DeleteEquipmentCommand)
/// </summary>
public class EquipmentNotFoundException : Exception
{
    public EquipmentNotFoundException(int id)
        : base($"Equipment with id '{id}' not found.")
    {
        EquipmentId = id;
    }

    public int EquipmentId { get; }
}

/// <summary>
///     Excepción lanzada cuando se intenta crear un equipo con un SerialNumber duplicado
///     Usado en: CommandService.Handle(CreateEquipmentCommand)
/// </summary>
public class DuplicateSerialNumberException : Exception
{
    public DuplicateSerialNumberException(string serialNumber)
        : base($"Equipment with serial number '{serialNumber}' already exists.")
    {
        SerialNumber = serialNumber;
    }

    public string SerialNumber { get; }
}

/// <summary>
///     Excepción lanzada cuando se intenta eliminar un equipo que está encendido
///     Usado en: CommandService.Handle(DeleteEquipmentCommand)
/// </summary>
public class EquipmentPoweredOnException : Exception
{
    public EquipmentPoweredOnException(string name)
        : base($"Cannot delete equipment '{name}' because it is powered on. Please turn it off first.")
    {
        EquipmentName = name;
    }

    public string EquipmentName { get; }
}

/// <summary>
///     Excepción lanzada cuando se intenta eliminar un equipo que está en mantenimiento
///     Usado en: CommandService.Handle(DeleteEquipmentCommand)
/// </summary>
public class EquipmentInMaintenanceException : Exception
{
    public EquipmentInMaintenanceException(string name)
        : base($"Cannot delete equipment '{name}' because it is under maintenance.")
    {
        EquipmentName = name;
    }

    public string EquipmentName { get; }
}

/// <summary>
///     Excepción lanzada cuando se intenta asignar un estado inválido a un equipo
///     Usado en: Equipment.UpdateStatus()
/// </summary>
public class InvalidStatusException : Exception
{
    public InvalidStatusException(string status)
        : base($"The status '{status}' is invalid. Status cannot be empty or whitespace.")
    {
        AttemptedStatus = status;
    }

    public string AttemptedStatus { get; }
}

/// <summary>
///     Excepción lanzada cuando se intenta crear una Location con datos inválidos
///     Usado en: Location constructor
/// </summary>
public class InvalidLocationException : Exception
{
    public InvalidLocationException(string message)
        : base($"Invalid location: {message}")
    {
    }

    // Factory methods para casos específicos
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

/// <summary>
///     Excepción lanzada cuando se intenta crear ControlSettings con datos inválidos
///     Usado en: ControlSettings constructor
/// </summary>
public class InvalidControlSettingsException : Exception
{
    public InvalidControlSettingsException(string message)
        : base($"Invalid control settings: {message}")
    {
    }

    // Factory methods para casos específicos
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

/// <summary>
///     Excepción lanzada cuando se intenta crear UsageStats con datos inválidos
///     Usado en: UsageStats constructor
/// </summary>
public class InvalidUsageStatsException : Exception
{
    public InvalidUsageStatsException(string message)
        : base($"Invalid usage stats: {message}")
    {
    }

    // Factory methods para casos específicos
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