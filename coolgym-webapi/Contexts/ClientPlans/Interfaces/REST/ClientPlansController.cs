using System.Net.Mime;
using coolgym_webapi.Contexts.ClientPlans.Domain.Queries;
using coolgym_webapi.Contexts.ClientPlans.Domain.Services;
using coolgym_webapi.Contexts.ClientPlans.Interfaces.REST.Transform;
using Microsoft.AspNetCore.Mvc;

namespace coolgym_webapi.Contexts.ClientPlans.Interfaces.REST;

/// <summary>
/// REST controller for client plans
/// </summary>
[ApiController]
[Route("api/v1/clientPlans")]
[Produces(MediaTypeNames.Application.Json)]
public class ClientPlansController(IClientPlanQueryService clientPlanQueryService) : ControllerBase
{
    /// <summary>
    /// Get all client plans
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAllPlans()
    {
        var query = new GetAllClientPlans();
        var plans = await clientPlanQueryService.Handle(query);
        var resources = plans.Select(ClientPlanResourceFromEntityAssembler.ToResourceFromEntity);
        return Ok(resources);
    }

    /// <summary>
    /// Get client plan by ID
    /// </summary>
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetPlanById(int id)
    {
        var query = new GetClientPlanById(id);
        var plan = await clientPlanQueryService.Handle(query);

        if (plan == null)
            return NotFound(new { message = $"Client plan with ID {id} not found" });

        var resource = ClientPlanResourceFromEntityAssembler.ToResourceFromEntity(plan);
        return Ok(resource);
    }
}
