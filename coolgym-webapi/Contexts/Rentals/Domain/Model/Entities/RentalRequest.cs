using coolgym_webapi.Contexts.Equipments.Domain.Model.Entities;
using coolgym_webapi.Contexts.Security.Domain.Model.Entities;
using coolgym_webapi.Contexts.Shared.Domain.Model.Entities;

namespace coolgym_webapi.Contexts.Rentals.Domain.Model.Entities;

/// <summary>
///     Represents a rental request made by a client for specific gym equipment
/// </summary>
public class RentalRequest : BaseEntity
{
    public RentalRequest()
    {
        Status = "pending";
        RequestDate = DateTime.UtcNow;
        MonthlyPrice = 0;
    }

    public RentalRequest(int equipmentId, int clientId, decimal monthlyPrice, string? notes = null)
    {
        EquipmentId = equipmentId;
        ClientId = clientId;
        MonthlyPrice = monthlyPrice;
        Notes = notes;
        Status = "pending";
        RequestDate = DateTime.UtcNow;
    }

    /// <summary>
    ///     Equipment identifier being requested for rental
    /// </summary>
    public int EquipmentId { get; set; }

    /// <summary>
    ///     Client identifier who is making the rental request
    /// </summary>
    public int ClientId { get; set; }

    /// <summary>
    ///     Provider identifier who accepted the rental request (null if pending)
    /// </summary>
    public int? ProviderId { get; set; }

    // Navigation properties
    public Equipment? Equipment { get; set; }
    public User? Client { get; set; }
    public User? Provider { get; set; }

    /// <summary>
    ///     Date when the request was made
    /// </summary>
    public DateTime RequestDate { get; set; }

    /// <summary>
    ///     Current status of the rental request
    ///     Valid values: "pending", "approved", "rejected", "completed"
    /// </summary>
    public string Status { get; set; }

    /// <summary>
    ///     Optional notes or comments about the rental request
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    ///     Monthly rental price at the time of request
    /// </summary>
    public decimal MonthlyPrice { get; set; }

    /// <summary>
    ///     Updates the status of the rental request
    /// </summary>
    public void UpdateStatus(string newStatus)
    {
        var validStatuses = new[] { "pending", "approved", "rejected", "completed", "cancelled" };
        if (!validStatuses.Contains(newStatus.ToLower()))
            throw new ArgumentException($"Invalid status: {newStatus}");

        Status = newStatus.ToLower();
        UpdatedDate = DateTime.UtcNow;
    }

    /// <summary>
    ///     Approves the rental request and assigns it to a provider
    /// </summary>
    public void Approve(int providerId)
    {
        if (Status != "pending")
            throw new InvalidOperationException("Only pending rental requests can be approved");

        ProviderId = providerId;
        Status = "approved";
        UpdatedDate = DateTime.UtcNow;
    }

    /// <summary>
    ///     Cancels the rental request (soft delete)
    /// </summary>
    public void Cancel()
    {
        Status = "cancelled";
        IsDeleted = 1;
        UpdatedDate = DateTime.UtcNow;
    }
}