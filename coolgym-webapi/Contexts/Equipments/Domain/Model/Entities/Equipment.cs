using coolgym_webapi.Contexts.Equipments.Domain;
using coolgym_webapi.Contexts.Equipments.Domain.Constants;
using coolgym_webapi.Contexts.Equipments.Domain.Exceptions;
using coolgym_webapi.Contexts.Equipments.Domain.Model.ValueObjects;
using coolgym_webapi.Contexts.Shared.Domain.Model.Entities;

namespace coolgym_webapi.Contexts.Equipments.Domain.Model.Entities;

public class Equipment : BaseEntity
{
    protected Equipment()
    {
        // Defaults used by EF when materializing the entity
        Status = EquipmentDomainConstants.StatusActive;
        ActiveStatus = EquipmentDomainConstants.DefaultControlStatus;
        Usage = new UsageStats();
        Controls = new ControlSettings(
            EquipmentDomainConstants.DefaultControlPower,
            EquipmentDomainConstants.DefaultControlInitialLevel,
            EquipmentDomainConstants.DefaultControlInitialLevel,
            EquipmentDomainConstants.DefaultControlMinLevel,
            EquipmentDomainConstants.DefaultControlMaxLevel,
            EquipmentDomainConstants.DefaultControlStatus);
        MaintenanceInfo = new MaintenanceInfo(null, null);
    }

    public Equipment(
        string name,
        string type,
        string model,
        string manufacturer,
        string serialNumber,
        string code,
        DateTime installationDate,
        int powerWatts,
        Location location)
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

        Status = EquipmentDomainConstants.StatusActive;
        IsPoweredOn = false;
        ActiveStatus = EquipmentDomainConstants.DefaultControlStatus;
        Image = null;

        Usage = new UsageStats();
        Controls = new ControlSettings(
            EquipmentDomainConstants.DefaultControlPower,
            EquipmentDomainConstants.DefaultControlInitialLevel,
            EquipmentDomainConstants.DefaultControlInitialLevel,
            EquipmentDomainConstants.DefaultControlMinLevel,
            EquipmentDomainConstants.DefaultControlMaxLevel,
            EquipmentDomainConstants.DefaultControlStatus);
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

    public string Status { get; set; } = EquipmentDomainConstants.StatusActive;
    public bool IsPoweredOn { get; set; }
    public string ActiveStatus { get; set; } = EquipmentDomainConstants.DefaultControlStatus;
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

    /// <summary>
    /// Turns the equipment on enforcing business rules:
    /// - Equipment cannot be turned on if it is in maintenance or inactive.
    /// </summary>
    public void TurnOn()
    {
        if (Status == EquipmentDomainConstants.StatusMaintenance ||
            Status == EquipmentDomainConstants.StatusInactive)
        {
            // We reuse InvalidStatusException so the controller can localize it.
            throw new InvalidStatusException(Status);
        }

        IsPoweredOn = true;
    }

    /// <summary>
    /// Turns the equipment off without extra rules.
    /// </summary>
    public void TurnOff()
    {
        IsPoweredOn = false;
    }

    /// <summary>
    /// Performs a soft delete of the equipment, enforcing:
    /// - It must not be powered on.
    /// - It must not be under maintenance.
    /// </summary>
    public void SoftDelete()
    {
        if (IsPoweredOn)
            throw new EquipmentPoweredOnException(Name);

        if (Status == EquipmentDomainConstants.StatusMaintenance)
            throw new EquipmentInMaintenanceException(Name);

        IsDeleted = EquipmentDomainConstants.DeletedFlagTrue;
        UpdatedDate = DateTime.UtcNow;
    }
}
