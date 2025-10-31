namespace coolgym_webapi.Contexts.Shared.Domain.Repositories;

public interface IBaseRepository<TEntity> where TEntity : class
{
    Task AddAsync(TEntity entity); // Añadir una nueva entidad

    Task<TEntity?> FindByIdAsync(int id); // Buscar una entidad por su ID

    void Update(TEntity entity); // Marcar una entidad para actualizar

    void Remove(TEntity entity); // Marcar una entidad para eliminar

    Task<IEnumerable<TEntity>> ListAsync(); // Obtener una lista de todas las entidades
}