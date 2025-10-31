using coolgym_webapi.Contexts.Equipments.Domain.Commands;
using coolgym_webapi.Contexts.Equipments.Domain.Model.Entities;

namespace coolgym_webapi.Contexts.Equipments.Domain.Services;

public interface IEquipmentCommandService
{
    Task<Equipment> Handle(CreateEquipmentCommand command);
    Task<Equipment?> Handle(UpdateEquipmentCommand command);
    //Elimina un equipo por su ID
    Task<bool> Handle(DeleteEquipmentCommand command); 
}