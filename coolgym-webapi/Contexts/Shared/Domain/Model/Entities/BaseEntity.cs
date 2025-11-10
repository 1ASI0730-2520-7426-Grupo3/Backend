namespace coolgym_webapi.Contexts.Shared.Domain.Model.Entities;

public abstract class BaseEntity
{
    public int Id { get; set; } // Clave primaria estándar

    public DateTime CreatedDate { get; set; } = DateTime.UtcNow; // Fecha de creación del registro

    public DateTime? UpdatedDate { get; set; } // Fecha de la última actualización

    public int IsDeleted { get; set; } = 0; // 0 = Activo, 1 = Eliminado
}