using coolgym_webapi.Contexts.Equipments.Domain.Exceptions;

namespace coolgym_webapi.Contexts.Equipments.Domain.Model.ValueObjects;

public record UsageStats
{
    public UsageStats(int totalMinutes, int todayMinutes, int caloriesToday)
    {
        if (totalMinutes < 0)
            throw InvalidUsageStatsException.NegativeTotalMinutes(totalMinutes);

        if (todayMinutes < 0)
            throw InvalidUsageStatsException.NegativeTodayMinutes(todayMinutes);

        if (caloriesToday < 0)
            throw InvalidUsageStatsException.NegativeCalories(caloriesToday);

        if (todayMinutes > totalMinutes)
            throw InvalidUsageStatsException.TodayExceedsTotal(todayMinutes, totalMinutes);

        TotalMinutes = totalMinutes;
        TodayMinutes = todayMinutes;
        CaloriesToday = caloriesToday;
    }

    // Constructor vacío para nuevos equipos
    public UsageStats() : this(0, 0, 0)
    {
    }

    public int TotalMinutes { get; init; }
    public int TodayMinutes { get; init; }
    public int CaloriesToday { get; init; }
}