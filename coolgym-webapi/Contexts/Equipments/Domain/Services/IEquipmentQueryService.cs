using coolgym_webapi.Contexts.Equipments.Domain.Model.Entities;
using coolgym_webapi.Contexts.Equipments.Domain.Queries;

namespace coolgym_webapi.Contexts.Equipments.Domain.Services;

public interface IEquipmentQueryService
{
    Task<IEnumerable<Equipment>> Handle(GetAllEquipment query);

    Task<Equipment?> Handle(GetEquipmentById query);

    Task<IEnumerable<Equipment>> Handle(GetEquipmentByType query);

    Task<IEnumerable<Equipment>> Handle(GetEquipmentByStatus query);
}