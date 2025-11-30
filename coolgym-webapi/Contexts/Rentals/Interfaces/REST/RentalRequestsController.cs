using System.Net.Mime;
using coolgym_webapi.Contexts.Rentals.Domain.Commands;
using coolgym_webapi.Contexts.Rentals.Domain.Queries;
using coolgym_webapi.Contexts.Rentals.Domain.Services;
using coolgym_webapi.Contexts.Rentals.Interfaces.REST.Resources;
using coolgym_webapi.Contexts.Rentals.Interfaces.REST.Transform;
using coolgym_webapi.Contexts.Security.Domain.Model.Entities;
using coolgym_webapi.Contexts.Security.Domain.Model.ValueObjects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace coolgym_webapi.Contexts.Rentals.Interfaces.REST;

/// <summary>
///     REST controller for managing rental requests
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Produces(MediaTypeNames.Application.Json)]
public class RentalRequestsController(
    IRentalRequestCommandService rentalRequestCommandService,
    IRentalRequestQueryService rentalRequestQueryService,
    IStringLocalizer<RentalRequestsController> localizer) : ControllerBase
{
    /// <summary>
    ///     Creates a new rental request
    /// </summary>
    /// <param name="resource">Rental request data</param>
    /// <returns>Created rental request</returns>
    [HttpPost]
    [ProducesResponseType(typeof(RentalRequestResource), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> CreateRentalRequest([FromBody] CreateRentalRequestResource resource)
    {
        try
        {
            // Get authenticated user from middleware
            var authenticatedUser = HttpContext.Items["User"] as User;

            // Authorization: User must be authenticated
            if (authenticatedUser == null)
                return Unauthorized(new { message = "Authentication required" });

            // Authorization: Only Clients can create rental requests
            if (authenticatedUser.Role.ToRoleName() != "Client")
                return StatusCode(403, new { message = "Only clients can create rental requests" });

            var command = CreateRentalRequestCommandFromResourceAssembler.ToCommandFromResource(resource);
            var rentalRequest = await rentalRequestCommandService.Handle(command);
            var rentalRequestResource = RentalRequestResourceFromEntityAssembler.ToResourceFromEntity(rentalRequest);

            return CreatedAtAction(
                nameof(GetRentalRequestById),
                new { id = rentalRequest.Id },
                rentalRequestResource);
        }
        catch (Exception ex)
        {
            return StatusCode(500,
                new { message = "An error occurred while creating the rental request", detail = ex.Message });
        }
    }

    /// <summary>
    ///     Gets all rental requests
    /// </summary>
    /// <returns>List of all rental requests</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<RentalRequestResource>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetAllRentalRequests()
    {
        // Get authenticated user from middleware
        var authenticatedUser = HttpContext.Items["User"] as User;

        // Authorization: User must be authenticated
        if (authenticatedUser == null)
            return Unauthorized(new { message = "Authentication required" });

        var query = new GetAllRentalRequests();
        var rentalRequests = await rentalRequestQueryService.Handle(query);
        var resources = RentalRequestResourceFromEntityAssembler.ToResourceFromEntity(rentalRequests);
        return Ok(resources);
    }

    /// <summary>
    ///     Gets a rental request by ID
    /// </summary>
    /// <param name="id">Rental request ID</param>
    /// <returns>Rental request details</returns>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(RentalRequestResource), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetRentalRequestById(int id)
    {
        var query = new GetRentalRequestById(id);
        var rentalRequest = await rentalRequestQueryService.Handle(query);

        if (rentalRequest == null)
            return NotFound(new { message = $"Rental request with id {id} not found" });

        var resource = RentalRequestResourceFromEntityAssembler.ToResourceFromEntity(rentalRequest);
        return Ok(resource);
    }

    /// <summary>
    ///     Gets rental requests by client ID
    /// </summary>
    /// <param name="clientId">Client ID</param>
    /// <returns>List of rental requests for the client</returns>
    [HttpGet("client/{clientId:int}")]
    [ProducesResponseType(typeof(IEnumerable<RentalRequestResource>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetRentalRequestsByClientId(int clientId)
    {
        var query = new GetRentalRequestsByClientId(clientId);
        var rentalRequests = await rentalRequestQueryService.Handle(query);
        var resources = RentalRequestResourceFromEntityAssembler.ToResourceFromEntity(rentalRequests);
        return Ok(resources);
    }

    /// <summary>
    ///     Updates the status of a rental request
    /// </summary>
    /// <param name="id">Rental request ID</param>
    /// <param name="resource">New status</param>
    /// <returns>Updated rental request</returns>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(RentalRequestResource), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> UpdateRentalRequestStatus(int id,
        [FromBody] UpdateRentalRequestStatusResource resource)
    {
        try
        {
            // Get authenticated user from middleware
            var authenticatedUser = HttpContext.Items["User"] as User;

            // Authorization: User must be authenticated
            if (authenticatedUser == null)
                return Unauthorized(new { message = "Authentication required" });

            // Authorization: Only Providers can update rental request status
            if (authenticatedUser.Role.ToRoleName() != "Provider")
                return StatusCode(403, new { message = "Only providers can update rental request status" });

            var command = new UpdateRentalRequestStatusCommand(id, resource.Status);
            var rentalRequest = await rentalRequestCommandService.Handle(command);

            if (rentalRequest == null)
                return NotFound(new { message = $"Rental request with id {id} not found" });

            var rentalRequestResource = RentalRequestResourceFromEntityAssembler.ToResourceFromEntity(rentalRequest);
            return Ok(rentalRequestResource);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500,
                new { message = "An error occurred while updating the rental request", detail = ex.Message });
        }
    }

    /// <summary>
    ///     Approves a rental request and auto-creates an invoice
    /// </summary>
    /// <param name="id">Rental request ID</param>
    /// <returns>Updated rental request with invoice created</returns>
    [HttpPost("{id:int}/approve")]
    [ProducesResponseType(typeof(RentalRequestResource), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ApproveRentalRequest(int id)
    {
        try
        {
            // Get authenticated user from middleware
            var authenticatedUser = HttpContext.Items["User"] as User;

            // Authorization: User must be authenticated
            if (authenticatedUser == null)
                return Unauthorized(new { message = "Authentication required" });

            // Authorization: Only Providers can approve rental requests
            if (authenticatedUser.Role.ToRoleName() != "Provider")
                return StatusCode(403, new { message = "Only providers can approve rental requests" });

            var command = new ApproveRentalRequestCommand(id, authenticatedUser.Id);
            var rentalRequest = await rentalRequestCommandService.Handle(command);

            if (rentalRequest == null)
                return NotFound(new { message = $"Rental request with id {id} not found" });

            var rentalRequestResource = RentalRequestResourceFromEntityAssembler.ToResourceFromEntity(rentalRequest);
            return Ok(rentalRequestResource);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500,
                new { message = "An error occurred while approving the rental request", detail = ex.Message });
        }
    }
}
