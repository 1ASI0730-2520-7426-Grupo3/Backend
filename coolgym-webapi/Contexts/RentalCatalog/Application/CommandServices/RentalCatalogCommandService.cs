using coolgym_webapi.Contexts.RentalCatalog.Domain.Commands;
using coolgym_webapi.Contexts.RentalCatalog.Domain.Exceptions;
using coolgym_webapi.Contexts.RentalCatalog.Domain.Model.Entities;
using coolgym_webapi.Contexts.RentalCatalog.Domain.Model.ValueObjects;
using coolgym_webapi.Contexts.RentalCatalog.Domain.Repositories;
using coolgym_webapi.Contexts.Shared.Domain.Repositories;

namespace coolgym_webapi.Contexts.RentalCatalog.Application.CommandServices;

public interface IRentalCatalogCommandService
{
    Task<RentalItem> Handle(CreateRentalItemCommand cmd);
    Task<RentalItem> Handle(UpdateRentalItemCommand cmd);
    Task Handle(DeleteRentalItemCommand cmd);
}

public class RentalCatalogCommandService : IRentalCatalogCommandService
{
    private readonly IRentalItemRepository _repo;
    private readonly IUnitOfWork _uow;

    public RentalCatalogCommandService(IRentalItemRepository repo, IUnitOfWork uow)
    {
        _repo = repo; _uow = uow;
    }

    public async Task<RentalItem> Handle(CreateRentalItemCommand cmd)
    {
        var exists = await _repo.ExistsByNameAndModelAsync(cmd.Name, cmd.Model);
        if (exists) throw new ArgumentException("Item already exists (name+model).");

        var price = new Money(cmd.MonthlyPriceUSD, string.IsNullOrWhiteSpace(cmd.Currency) ? "USD" : cmd.Currency);
        var entity = new RentalItem(cmd.Name, cmd.Type, cmd.Model, price, cmd.ImageUrl, cmd.IsAvailable);

        await _repo.AddAsync(entity);
        await _uow.CompleteAsync();
        return entity;
    }

    public async Task<RentalItem> Handle(UpdateRentalItemCommand cmd)
    {
        var entity = await _repo.FindByIdAsync(cmd.Id) ?? throw new RentalItemNotFoundException(cmd.Id);
        entity.UpdateBasicInfo(cmd.Name, cmd.Type, cmd.Model, cmd.ImageUrl);
        entity.UpdateMonthlyPrice(new Money(cmd.MonthlyPriceUSD, string.IsNullOrWhiteSpace(cmd.Currency) ? "USD" : cmd.Currency));
        entity.SetAvailability(cmd.IsAvailable);

        _repo.Update(entity);
        await _uow.CompleteAsync();
        return entity;
    }

    public async Task Handle(DeleteRentalItemCommand cmd)
    {
        var entity = await _repo.FindByIdAsync(cmd.Id) ?? throw new RentalItemNotFoundException(cmd.Id);
        _repo.Remove(entity); // si usas soft delete, marca IsDeleted en BaseRepository.Remove
        await _uow.CompleteAsync();
    }
}