namespace coolgym_webapi.Contexts.Shared.Domain.Repositories;

public interface IUnitOfWork
{
    Task CompleteAsync();
}