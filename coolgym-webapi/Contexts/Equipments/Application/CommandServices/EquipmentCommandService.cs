using coolgym_webapi.Contexts.Equipments.Domain.Commands;
using coolgym_webapi.Contexts.Equipments.Domain.Exceptions;
using coolgym_webapi.Contexts.Equipments.Domain.Model.Entities;
using coolgym_webapi.Contexts.Equipments.Domain.Model.ValueObjects;
using coolgym_webapi.Contexts.Equipments.Domain.Repositories;
using coolgym_webapi.Contexts.Equipments.Domain.Services;
using coolgym_webapi.Contexts.Shared.Domain.Repositories;

namespace coolgym_webapi.Contexts.Equipments.Application.CommandServices;

/// <summary>
///     Application service for Equipment commands (Write operations)
///     Implements business logic for CREATE, UPDATE, DELETE
/// </summary>
public class EquipmentCommandService(IEquipmentRepository equipmentRepository, IUnitOfWork unitOfWork)
    : IEquipmentCommandService
{
    /// <summary>
    ///     Handles the command to create new equipment
    /// </summary>
    /// <param name="command">Equipment data to create</param>
    /// <returns>Created equipment</returns>
    public async Task<Equipment> Handle(CreateEquipmentCommand command)
    {
        var existingEquipment = await equipmentRepository.FindBySerialNumberAsync(command.SerialNumber);
        if (existingEquipment != null) throw new DuplicateSerialNumberException(command.SerialNumber);

        var location = new Location(command.LocationName, command.LocationAddress);

        var equipment = new Equipment(
            command.Name,
            command.Type,
            command.Model,
            command.Manufacturer,
            command.SerialNumber,
            command.Code,
            command.InstallationDate,
            command.PowerWatts,
            location
        );

        if (!string.IsNullOrWhiteSpace(command.Image)) equipment.UpdateImage(command.Image);

        await equipmentRepository.AddAsync(equipment);
        await unitOfWork.CompleteAsync();

        return equipment;
    }

    /// <summary>
    ///     Handles the command to update existing equipment
    /// </summary>
    /// <param name="command">Equipment data to update</param>
    /// <returns>Updated equipment or null if not found</returns>
    public async Task<Equipment?> Handle(UpdateEquipmentCommand command)
    {
        var equipment = await equipmentRepository.FindByIdAsync(command.Id);
        if (equipment == null)
            throw new EquipmentNotFoundException(command.Id);

        equipment.Name = command.Name;
        equipment.Code = command.Code;
        equipment.PowerWatts = command.PowerWatts;
        equipment.IsPoweredOn = command.IsPoweredOn;
        equipment.ActiveStatus = command.ActiveStatus;
        equipment.Notes = command.Notes;

        equipment.UpdateStatus(command.Status);

        var newLocation = new Location(command.LocationName, command.LocationAddress);
        equipment.UpdateLocation(newLocation);

        equipment.UpdateImage(command.Image);
        equipment.UpdatedDate = DateTime.UtcNow;

        equipmentRepository.Update(equipment);
        await unitOfWork.CompleteAsync();

        return equipment;
    }

    /// <summary>
    ///     Handles the command to delete equipment
    /// </summary>
    /// <param name="command">ID of equipment to delete</param>
    /// <returns>True if deleted successfully, False if not found</returns>
    public async Task<bool> Handle(DeleteEquipmentCommand command)
    {
        var equipment = await equipmentRepository.FindByIdAsync(command.Id);

        if (equipment == null || equipment.IsDeleted == 1)
            throw new EquipmentNotFoundException(command.Id);

        if (equipment.IsPoweredOn)
            throw new EquipmentPoweredOnException(equipment.Name);

        if (equipment.Status == "maintenance")
            throw new EquipmentInMaintenanceException(equipment.Name);

        equipment.IsDeleted = 1;
        equipment.UpdatedDate = DateTime.UtcNow;

        equipmentRepository.Update(equipment);
        await unitOfWork.CompleteAsync();

        return true;
    }
}