using coolgym_webapi.Contexts.RentalCatalog.Application.CommandServices;
using coolgym_webapi.Contexts.RentalCatalog.Application.QueryServices;
using coolgym_webapi.Contexts.RentalCatalog.Domain.Commands;
using coolgym_webapi.Contexts.RentalCatalog.Domain.Queries;
using coolgym_webapi.Contexts.RentalCatalog.Interfaces.REST.Resources;
using coolgym_webapi.Contexts.RentalCatalog.Interfaces.REST.Transform;
using Microsoft.AspNetCore.Mvc;

namespace coolgym_webapi.Contexts.RentalCatalog.Interfaces.REST;

[ApiController]
[Route("api/v1/[controller]")]
public class RentalItemsController : ControllerBase
{
    private readonly IRentalCatalogCommandService _cmds;
    private readonly IRentalCatalogQueryService _qs;

    public RentalItemsController(IRentalCatalogCommandService cmds, IRentalCatalogQueryService qs)
    {
        _cmds = cmds; _qs = qs;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll() =>
        Ok((await _qs.Handle(new GetAllRentalItemsQuery())).Select(RentalItemAssemblers.ToResource));

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var item = await _qs.Handle(new GetRentalItemByIdQuery(id));
        return item is null ? NotFound() : Ok(RentalItemAssemblers.ToResource(item));
    }

    [HttpGet("type/{type}")]
    public async Task<IActionResult> GetByType(string type) =>
        Ok((await _qs.Handle(new GetRentalItemsByTypeQuery(type))).Select(RentalItemAssemblers.ToResource));

    [HttpGet("available")]
    public async Task<IActionResult> GetAvailable() =>
        Ok((await _qs.Handle(new GetAvailableRentalItemsQuery())).Select(RentalItemAssemblers.ToResource));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateRentalItemResource r)
    {
        var created = await _cmds.Handle(r.ToCommand());
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, RentalItemAssemblers.ToResource(created));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateRentalItemResource r)
    {
        if (id != r.Id) return BadRequest("Route id and body id must match");
        var updated = await _cmds.Handle(r.ToCommand());
        return Ok(RentalItemAssemblers.ToResource(updated));
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _cmds.Handle(new DeleteRentalItemCommand(id));
        return NoContent();
    }
}