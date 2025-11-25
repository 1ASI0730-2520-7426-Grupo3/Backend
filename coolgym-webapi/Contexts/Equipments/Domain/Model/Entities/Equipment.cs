using coolgym_webapi.Contexts.Equipments.Domain.Exceptions;
using coolgym_webapi.Contexts.Equipments.Domain.Model.ValueObjects;
using coolgym_webapi.Contexts.Shared.Domain.Model.Entities;

namespace coolgym_webapi.Contexts.Equipments.Domain.Model.Entities;

public class Equipment : BaseEntity
{
    protected Equipment()
    {
    }

    public Equipment(string name, string type, string model, string manufacturer, string serialNumber, string code,
        DateTime installationDate, int powerWatts, Location location)
    {
        Name = name;
        Type = type;
        Model = model;
        Manufacturer = manufacturer;
        SerialNumber = serialNumber;
        Code = code;
        InstallationDate = installationDate;
        PowerWatts = powerWatts;
        Location = location ?? throw new ArgumentNullException(nameof(location));
        Status = "active";
        IsPoweredOn = false;
        ActiveStatus = "Normal";
        Image = null;

        Usage = new UsageStats(0, 0, 0);

        Controls = new ControlSettings("off", 1, 1, 1, 10, "Normal");
        MaintenanceInfo = new MaintenanceInfo(null, null);
    }

    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public string Manufacturer { get; set; } = string.Empty;
    public string SerialNumber { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public DateTime InstallationDate { get; set; }
    public int PowerWatts { get; set; }
    public string Status { get; set; } = "active";
    public bool IsPoweredOn { get; set; }
    public string ActiveStatus { get; set; } = string.Empty;
    public string? Notes { get; set; }

    public string? Image { get; set; }

    public Location Location { get; private set; } = null!;
    public UsageStats Usage { get; private set; } = null!;
    public ControlSettings Controls { get; private set; } = null!;
    public MaintenanceInfo MaintenanceInfo { get; private set; } = null!;

    public void UpdateStatus(string newStatus)
    {
        if (string.IsNullOrWhiteSpace(newStatus))
            throw new InvalidStatusException(newStatus);
        Status = newStatus;
    }

    public void UpdateLocation(Location newLocation)
    {
        ArgumentNullException.ThrowIfNull(newLocation);
        Location = newLocation;
    }

    public void RecordMaintenance(DateTime lastMaintenanceDate, DateTime? nextMaintenanceDate = null)
    {
        MaintenanceInfo = new MaintenanceInfo(lastMaintenanceDate, nextMaintenanceDate);
    }

    public void UpdateImage(string? imageUrl)
    {
        Image = imageUrl;
    }
}