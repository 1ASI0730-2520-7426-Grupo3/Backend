using System.Net.Mime;
using coolgym_webapi.Contexts.maintenance.Domain.Queries;
using coolgym_webapi.Contexts.maintenance.Domain.Services;
using coolgym_webapi.Contexts.maintenance.Interfaces.REST.Resources;
using coolgym_webapi.Contexts.maintenance.Interfaces.REST.Transform;
using Microsoft.AspNetCore.Mvc;

namespace coolgym_webapi.Contexts.maintenance.Interfaces.REST;

[ApiController]
[Route("api/v1/[controller]")]
[Produces(MediaTypeNames.Application.Json)]
public class MaintenanceRequestsController(IMaintenanceRequestCommandService maintenanceRequestCommandService,
    IMaintenanceRequestQueryService maintenanceRequestQueryService) : ControllerBase
{
    
    [HttpPost]
    [ProducesResponseType(typeof(MaintenanceRequestResource), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateEquipment([FromBody] CreateMaintenanceRequestResource resource)
    {
        try
        {
            var command = CreateMaintenanceRequestCommandFromResourceAssembler.ToCommandFromResource(resource);
            var maintenanceRequest = await maintenanceRequestCommandService.Handle(command);
            var maintenanceRequestResource = MaintenanceRequestResourceFromEntityAssembler.ToResourceFromEntity(maintenanceRequest);

            return CreatedAtAction(
                nameof(GetMaintenanceRequestById),
                new { id = maintenanceRequest.Id },
                maintenanceRequestResource
            );
        }
        catch (InvalidDataException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500,
                new { message = "An error occurred while creating the Maintenance Request", detail = ex.Message });
        }
    }

    
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<MaintenanceRequestResource>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllMaintenanceRequests()
    {
        var query = new GetAllMaintenanceRequests();
        var maintenanceRequests = await maintenanceRequestQueryService.Handle(query);
        var resources = MaintenanceRequestResourceFromEntityAssembler.ToResourceFromEntity(maintenanceRequests);
        return Ok(resources);
    }
    
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(MaintenanceRequestResource), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetMaintenanceRequestById(int id)
    {
        var query = new GetMaintenanceRequestById(id);
        var maintenanceRequest = await maintenanceRequestQueryService.Handle(query);

        if (maintenanceRequest == null)
            return NotFound(new { message = $"Maintenance Request with id {id} not found" });

        var resource = MaintenanceRequestResourceFromEntityAssembler.ToResourceFromEntity(maintenanceRequest);
        return Ok(resource);
    }
    
    
    [HttpGet("status/{status}")]
    [ProducesResponseType(typeof(IEnumerable<MaintenanceRequestResource>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMaintenanceRequestsByStatus(string status)
    {
        var query = new GetMaintenanceRequestsByStatus(status);
        var maintenanceRequests = await maintenanceRequestQueryService.Handle(query);
        var resources = MaintenanceRequestResourceFromEntityAssembler.ToResourceFromEntity(maintenanceRequests);
        return Ok(resources);
    }
    
    
    
}