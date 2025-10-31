namespace coolgym_webapi.Contexts.Equipments.Domain.Model.ValueObjects;

public record UsageStats
{
    public int TotalMinutes { get; init; }
    public int TodayMinutes { get; init; }
    public int CaloriesToday { get; init; }

    public UsageStats(int totalMinutes, int todayMinutes, int caloriesToday)
    {
        // Validación: No permitir valores negativos
        if (totalMinutes < 0)
            throw new ArgumentException("Los minutos totales no pueden ser negativos.", nameof(totalMinutes));
        
        if (todayMinutes < 0)
            throw new ArgumentException("Los minutos de hoy no pueden ser negativos.", nameof(todayMinutes));
        
        if (caloriesToday < 0)
            throw new ArgumentException("Las calorías de hoy no pueden ser negativas.", nameof(caloriesToday));

        // Validación: Los minutos de hoy no pueden superar el total
        if (todayMinutes > totalMinutes)
            throw new ArgumentException(
                "Los minutos de hoy no pueden ser mayores que los minutos totales.", 
                nameof(todayMinutes));

        TotalMinutes = totalMinutes;
        TodayMinutes = todayMinutes;
        CaloriesToday = caloriesToday;
    }

    // Constructor vacío para nuevos equipos
    public UsageStats() : this(0, 0, 0) { }
}