using coolgym_webapi.Contexts.Shared.Domain.Model.Entities;
using InvalidDataException = coolgym_webapi.Contexts.maintenance.Domain.Exceptions.InvalidDataException;

namespace coolgym_webapi.Contexts.maintenance.Domain.Model.Entities;

public class MaintenanceRequest : BaseEntity
{
    protected MaintenanceRequest() { }
    
    public int EquipmentId { get; set; }
    public DateTime SelectedDate { get; set; }
    public string Observation { get; set; }
    public string Status { get; set; }

    public MaintenanceRequest(int equipmentId, DateTime selectedDate, string observation)
    {
        this.EquipmentId = equipmentId;
        this.SelectedDate = selectedDate;
        this.Observation = observation;
        Status = "Pending";
    }
    
    public void UpdateStatus(string newStatus)
    {
        if (string.IsNullOrWhiteSpace(newStatus))
            throw new InvalidDataException(newStatus);
        Status = newStatus;
    }
    
    
}