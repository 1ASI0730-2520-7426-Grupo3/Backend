namespace coolgym_webapi.Contexts.Shared.Domain.Model.Entities;

public abstract class BaseEntity
{
    public int Id { get; set; }

    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedDate { get; set; }

    public int IsDeleted { get; set; } = 0;
}