using coolgym_webapi.Contexts.Equipments.Domain.Commands;
using coolgym_webapi.Contexts.Equipments.Domain.Model.Entities;
using coolgym_webapi.Contexts.Equipments.Domain.Model.ValueObjects;
using coolgym_webapi.Contexts.Equipments.Domain.Repositories;
using coolgym_webapi.Contexts.Equipments.Domain.Services;
using coolgym_webapi.Contexts.Shared.Domain.Repositories;

namespace coolgym_webapi.Contexts.Equipments.Application.CommandServices;

/// <summary>
/// Servicio de aplicación para comandos de Equipment (Operaciones de escritura)
/// Implementa la lógica de negocio para CREATE, UPDATE, DELETE
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
    /// Maneja el comando para crear un nuevo equipo
    /// </summary>
    /// <param name="command">Datos del equipo a crear</param>
    /// <returns>El equipo creado</returns>
    public async Task<Equipment> Handle(CreateEquipmentCommand command)
    {
        // Validar que el SerialNumber no exista
        var existingEquipment = await _equipmentRepository.FindBySerialNumberAsync(command.SerialNumber);
        if (existingEquipment != null)
        {
            throw new InvalidOperationException(
                $"Ya existe un equipo con el número de serie '{command.SerialNumber}'.");
        }
        
        var location = new Location(command.LocationName, command.LocationAddress);
        
        var equipment = new Equipment(
            name: command.Name,
            type: command.Type,
            model: command.Model,
            manufacturer: command.Manufacturer,
            serialNumber: command.SerialNumber,
            code: command.Code,
            installationDate: command.InstallationDate,
            powerWatts: command.PowerWatts,
            location: location
        );
        
        if (!string.IsNullOrWhiteSpace(command.Image))
        {
            equipment.UpdateImage(command.Image);
        }
        
        await _equipmentRepository.AddAsync(equipment);
        await _unitOfWork.CompleteAsync();

        return equipment;
    }

    /// <summary>
    /// Maneja el comando para actualizar un equipo existente
    /// </summary>
    /// <param name="command">Datos del equipo a actualizar</param>
    /// <returns>El equipo actualizado o null si no existe</returns>
    public async Task<Equipment?> Handle(UpdateEquipmentCommand command)
    {
        // Buscar el equipo existente
        var equipment = await _equipmentRepository.FindByIdAsync(command.Id);
        if (equipment == null)
            return null;

        // Actualizar propiedades simples directamente
        equipment.Name = command.Name;
        equipment.Code = command.Code;
        equipment.PowerWatts = command.PowerWatts;
        equipment.IsPoweredOn = command.IsPoweredOn;
        equipment.ActiveStatus = command.ActiveStatus;
        equipment.Notes = command.Notes;

        // Actualizar Status usando el método de dominio
        equipment.UpdateStatus(command.Status);

        // Actualizar Location usando el método de dominio
        var newLocation = new Location(command.LocationName, command.LocationAddress);
        equipment.UpdateLocation(newLocation);
        
        // Actualizar imagen
        equipment.UpdateImage(command.Image);

        // Actualizar UpdatedDate
        equipment.UpdatedDate = DateTime.UtcNow;

        // Guardar cambios
        _equipmentRepository.Update(equipment);
        await _unitOfWork.CompleteAsync();

        return equipment;
    }

    /// <summary>
    /// Maneja el comando para eliminar un equipo
    /// </summary>
    /// <param name="command">ID del equipo a eliminar</param>
    /// <returns>True si se eliminó correctamente, False si no existe</returns>
    public async Task<bool> Handle(DeleteEquipmentCommand command)
    {
        // Buscar el equipo
        var equipment = await _equipmentRepository.FindByIdAsync(command.Id);
        if (equipment == null)
            return false;

        // Validaciones de reglas de negocio antes de eliminar
        if (equipment.IsPoweredOn)
        {
            throw new InvalidOperationException(
                $"No se puede eliminar el equipo '{equipment.Name}' porque está encendido. Apáguelo primero.");
        }

        if (equipment.Status == "maintenance")
        {
            throw new InvalidOperationException(
                $"No se puede eliminar el equipo '{equipment.Name}' porque está en mantenimiento.");
        }

        // Eliminar el equipo
        _equipmentRepository.Remove(equipment);
        await _unitOfWork.CompleteAsync();

        return true;
    }
}