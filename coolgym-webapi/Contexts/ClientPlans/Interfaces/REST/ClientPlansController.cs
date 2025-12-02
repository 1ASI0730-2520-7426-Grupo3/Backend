using System.Net.Mime;
using coolgym_webapi.Contexts.ClientPlans.Domain.Queries;
using coolgym_webapi.Contexts.ClientPlans.Domain.Services;
using coolgym_webapi.Contexts.ClientPlans.Interfaces.REST.Transform;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace coolgym_webapi.Contexts.ClientPlans.Interfaces.REST;

/// <summary>
///     REST controller that exposes read-only endpoints for client subscription plans.
/// </summary>
/// <remarks>
///     Responses from this controller return plan information. Error messages
///     are localized according to the <c>Accept-Language</c> HTTP header.
/// </remarks>
[ApiController]
[Route("api/v1/clientPlans")]
[Produces(MediaTypeNames.Application.Json)]
public class ClientPlansController(
    IClientPlanQueryService clientPlanQueryService,
    IStringLocalizer<ClientPlansController> localizer) : ControllerBase
{
    /// <summary>
    ///     Gets all client subscription plans.
    /// </summary>
    /// <remarks>
    ///     This endpoint is typically used by the pricing page in the public
    ///     site or by the mobile app when showing the list of available plans.
    /// </remarks>
    /// <returns>Collection of client plans.</returns>
    /// <response code="200">Returns the full list of client plans.</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllPlans()
    {
        var query = new GetAllClientPlans();
        var plans = await clientPlanQueryService.Handle(query);
        var resources = plans.Select(plan => ClientPlanResourceFromEntityAssembler.ToResourceFromEntity(
            plan,
            localizer[$"Plan.{plan.Name}.Name"].Value,
            localizer[$"Plan.{plan.Name}.Description"].Value));
        return Ok(resources);
    }

    /// <summary>
    ///     Gets a client plan by its identifier.
    /// </summary>
    /// <param name="id">Client plan identifier.</param>
    /// <returns>The requested client plan, if it exists.</returns>
    /// <response code="200">Client plan found.</response>
    /// <response code="404">No client plan exists with the given identifier.</response>
    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPlanById(int id)
    {
        var query = new GetClientPlanById(id);
        var plan = await clientPlanQueryService.Handle(query);

        if (plan == null)
            return NotFound(new { message = localizer["ClientPlanNotFound", id].Value });

        var resource = ClientPlanResourceFromEntityAssembler.ToResourceFromEntity(
            plan,
            localizer[$"Plan.{plan.Name}.Name"].Value,
            localizer[$"Plan.{plan.Name}.Description"].Value);
        return Ok(resource);
    }
}