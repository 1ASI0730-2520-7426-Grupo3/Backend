using coolgym_webapi.Contexts.Equipments.Domain.Commands;
using coolgym_webapi.Contexts.Equipments.Domain.Exceptions;
using coolgym_webapi.Contexts.Equipments.Domain.Model.Entities;
using coolgym_webapi.Contexts.Equipments.Domain.Model.ValueObjects;
using coolgym_webapi.Contexts.Equipments.Domain.Repositories;
using coolgym_webapi.Contexts.Equipments.Domain.Services;
using coolgym_webapi.Contexts.Shared.Domain.Repositories;

namespace coolgym_webapi.Contexts.Equipments.Application.CommandServices;

/// <summary>
///     Servicio de aplicación para comandos de Equipment (Operaciones de escritura)
///     Implementa la lógica de negocio para CREATE, UPDATE, DELETE
/// </summary>
public class EquipmentCommandService : IEquipmentCommandService
{
    private readonly IEquipmentRepository _equipmentRepository;
    private readonly IUnitOfWork _unitOfWork;

    public EquipmentCommandService(
        IEquipmentRepository equipmentRepository,
        IUnitOfWork unitOfWork)
    {
        _equipmentRepository = equipmentRepository;
        _unitOfWork = unitOfWork;
    }

    /// <summary>
    ///     Maneja el comando para crear un nuevo equipo
    /// </summary>
    /// <param name="command">Datos del equipo a crear</param>
    /// <returns>El equipo creado</returns>
    public async Task<Equipment> Handle(CreateEquipmentCommand command)
    {
        var existingEquipment = await _equipmentRepository.FindBySerialNumberAsync(command.SerialNumber);
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

        await _equipmentRepository.AddAsync(equipment);
        await _unitOfWork.CompleteAsync();

        return equipment;
    }

    /// <summary>
    ///     Maneja el comando para actualizar un equipo existente
    /// </summary>
    /// <param name="command">Datos del equipo a actualizar</param>
    /// <returns>El equipo actualizado o null si no existe</returns>
    public async Task<Equipment?> Handle(UpdateEquipmentCommand command)
    {
        var equipment = await _equipmentRepository.FindByIdAsync(command.Id);
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

        _equipmentRepository.Update(equipment);
        await _unitOfWork.CompleteAsync();

        return equipment;
    }

    /// <summary>
    ///     Maneja el comando para eliminar un equipo
    /// </summary>
    /// <param name="command">ID del equipo a eliminar</param>
    /// <returns>True si se eliminó correctamente, False si no existe</returns>
    public async Task<bool> Handle(DeleteEquipmentCommand command)
    {
        var equipment = await _equipmentRepository.FindByIdAsync(command.Id);
        if (equipment == null)
            throw new EquipmentNotFoundException(command.Id);

        if (equipment.IsPoweredOn) throw new EquipmentPoweredOnException(equipment.Name);

        if (equipment.Status == "maintenance") throw new EquipmentInMaintenanceException(equipment.Name);

        _equipmentRepository.Remove(equipment);
        await _unitOfWork.CompleteAsync();

        return true;
    }
}