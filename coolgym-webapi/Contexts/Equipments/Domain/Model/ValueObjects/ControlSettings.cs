using coolgym_webapi.Contexts.Equipments.Domain.Exceptions;

namespace coolgym_webapi.Contexts.Equipments.Domain.Model.ValueObjects;

public record ControlSettings
{
    public ControlSettings(
        string power,
        int currentLevel,
        int setLevel,
        int minLevelRange,
        int maxLevelRange,
        string status)
    {
        if (string.IsNullOrWhiteSpace(power))
            throw InvalidControlSettingsException.EmptyPower();

        if (string.IsNullOrWhiteSpace(status))
            throw InvalidControlSettingsException.EmptyStatus();

        if (minLevelRange < 0)
            throw InvalidControlSettingsException.NegativeMinLevel(minLevelRange);

        if (maxLevelRange <= minLevelRange)
            throw InvalidControlSettingsException.InvalidRange(minLevelRange, maxLevelRange);

        if (currentLevel < minLevelRange || currentLevel > maxLevelRange)
            throw InvalidControlSettingsException.CurrentLevelOutOfRange(
                currentLevel, minLevelRange, maxLevelRange);

        if (setLevel < minLevelRange || setLevel > maxLevelRange)
            throw InvalidControlSettingsException.SetLevelOutOfRange(
                setLevel, minLevelRange, maxLevelRange);

        Power = power;
        CurrentLevel = currentLevel;
        SetLevel = setLevel;
        MinLevelRange = minLevelRange;
        MaxLevelRange = maxLevelRange;
        Status = status;
    }

    public string Power { get; init; }
    public int CurrentLevel { get; init; }
    public int SetLevel { get; init; }
    public int MinLevelRange { get; init; }
    public int MaxLevelRange { get; init; }
    public string Status { get; init; }
}