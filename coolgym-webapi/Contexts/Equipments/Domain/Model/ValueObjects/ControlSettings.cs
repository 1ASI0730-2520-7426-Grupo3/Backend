namespace coolgym_webapi.Contexts.Equipments.Domain.Model.ValueObjects;

public record ControlSettings
{
    public string Power { get; init; }
    public int CurrentLevel { get; init; }
    public int SetLevel { get; init; }
    public int MinLevelRange { get; init; }
    public int MaxLevelRange { get; init; }
    public string Status { get; init; }

    public ControlSettings(
        string power, 
        int currentLevel, 
        int setLevel, 
        int minLevelRange, 
        int maxLevelRange, 
        string status)
    {
        // Validación de strings
        if (string.IsNullOrWhiteSpace(power))
            throw new ArgumentException("El estado de encendido no puede estar vacío.", nameof(power));
        
        if (string.IsNullOrWhiteSpace(status))
            throw new ArgumentException("El estado no puede estar vacío.", nameof(status));

        // Validación de rangos
        if (minLevelRange < 0)
            throw new ArgumentException("El nivel mínimo no puede ser negativo.", nameof(minLevelRange));
        
        if (maxLevelRange <= minLevelRange)
            throw new ArgumentException(
                $"El nivel máximo ({maxLevelRange}) debe ser mayor que el mínimo ({minLevelRange}).", 
                nameof(maxLevelRange));
        
        // Validación de niveles actuales
        if (currentLevel < minLevelRange || currentLevel > maxLevelRange)
            throw new ArgumentException(
                $"El nivel actual ({currentLevel}) debe estar entre {minLevelRange} y {maxLevelRange}.", 
                nameof(currentLevel));
        
        if (setLevel < minLevelRange || setLevel > maxLevelRange)
            throw new ArgumentException(
                $"El nivel configurado ({setLevel}) debe estar entre {minLevelRange} y {maxLevelRange}.", 
                nameof(setLevel));

        Power = power;
        CurrentLevel = currentLevel;
        SetLevel = setLevel;
        MinLevelRange = minLevelRange;
        MaxLevelRange = maxLevelRange;
        Status = status;
    }
}