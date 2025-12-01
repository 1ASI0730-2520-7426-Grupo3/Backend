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
///     REST controller for managing rental requests between clients and providers.
/// </summary>
/// <remarks>
///     All messages returned by this controller are localized using the
///     <c>Accept-Language</c> HTTP header (currently supporting <c>en</c> and <c>es</c>).
///     <para>
///         Authentication is resolved from <c>HttpContext.Items["User"]</c>, which is
///         populated by the JWT middleware.
///     </para>
/// </remarks>
[ApiController]
[Route("api/v1/[controller]")]
[Produces(MediaTypeNames.Application.Json)]
public class RentalRequestsController(
    IRentalRequestCommandService rentalRequestCommandService,
    IRentalRequestQueryService rentalRequestQueryService,
    IStringLocalizer<RentalRequestsController> localizer) : ControllerBase
{
    /// <summary>
    ///     Creates a new rental request.
    /// </summary>
    /// <remarks>
    ///     Business rules:
    ///     <list type="bullet">
    ///         <item><description>The caller must be authenticated.</description></item>
    ///         <item><description>Only users with role <c>Client</c> can create rental requests.</description></item>
    ///         <item><description>A client cannot create more than one pending request for the same equipment.</description></item>
    ///     </list>
    ///
    ///     <para>Sample request body:</para>
    ///     <code language="json">
    ///     POST /api/v1/RentalRequests
    ///     {
    ///       "equipmentId": 5,
    ///       "clientId": 12,
    ///       "monthlyPrice": 350.00,
    ///       "notes": "Requesting 6-month rental for cardio area."
    ///     }
    ///     </code>
    /// </remarks>
    /// <param name="resource">Rental request data sent by the client.</param>
    /// <returns>The created rental request resource.</returns>
    /// <response code="201">Rental request created successfully.</response>
    /// <response code="400">Domain validation failed (e.g., duplicate pending request).</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User is authenticated but not a client.</response>
    /// <response code="500">Unexpected error while creating the rental request.</response>
    [HttpPost]
    [ProducesResponseType(typeof(RentalRequestResource), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> CreateRentalRequest([FromBody] CreateRentalRequestResource resource)
    {
        try
        {
            // Get authenticated user from middleware
            var authenticatedUser = HttpContext.Items["User"] as User;

            // Authorization: User must be authenticated
            if (authenticatedUser == null)
                return Unauthorized(new { message = localizer["AuthenticationRequired"].Value });

            // Authorization: Only Clients can create rental requests
            if (authenticatedUser.Role.ToRoleName() != "Client")
                return StatusCode(StatusCodes.Status403Forbidden,
                    new { message = localizer["OnlyClientsCanCreateRentalRequests"].Value });

            var command = CreateRentalRequestCommandFromResourceAssembler.ToCommandFromResource(resource);
            var rentalRequest = await rentalRequestCommandService.Handle(command);
            var rentalRequestResource = RentalRequestResourceFromEntityAssembler.ToResourceFromEntity(rentalRequest);

            return CreatedAtAction(
                nameof(GetRentalRequestById),
                new { id = rentalRequest.Id },
                rentalRequestResource);
        }
        catch (InvalidOperationException ex)
        {
            // e.g. "A pending rental request already exists for this equipment"
            var message = localizer["InvalidRentalOperation", ex.Message].Value;
            return BadRequest(new { message });
        }
        catch (Exception ex)
        {
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                new
                {
                    message = localizer["ErrorCreatingRentalRequest"].Value,
                    detail = ex.Message
                });
        }
    }

    /// <summary>
    ///     Gets all rental requests in the system.
    /// </summary>
    /// <remarks>
    ///     This endpoint is typically used by back-office or provider dashboards to review
    ///     all rental requests, regardless of their status.
    /// </remarks>
    /// <returns>List of all rental requests.</returns>
    /// <response code="200">Returns the list of rental requests.</response>
    /// <response code="401">User is not authenticated.</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<RentalRequestResource>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetAllRentalRequests()
    {
        // Get authenticated user from middleware
        var authenticatedUser = HttpContext.Items["User"] as User;

        // Authorization: User must be authenticated
        if (authenticatedUser == null)
            return Unauthorized(new { message = localizer["AuthenticationRequired"].Value });

        var query = new GetAllRentalRequests();
        var rentalRequests = await rentalRequestQueryService.Handle(query);
        var resources = RentalRequestResourceFromEntityAssembler.ToResourceFromEntity(rentalRequests);
        return Ok(resources);
    }

    /// <summary>
    ///     Gets a rental request by its identifier.
    /// </summary>
    /// <param name="id">Rental request identifier.</param>
    /// <returns>Rental request details, if found.</returns>
    /// <response code="200">Rental request found.</response>
    /// <response code="404">No rental request exists with the given identifier.</response>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(RentalRequestResource), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetRentalRequestById(int id)
    {
        var query = new GetRentalRequestById(id);
        var rentalRequest = await rentalRequestQueryService.Handle(query);

        if (rentalRequest == null)
            return NotFound(new { message = localizer["RentalRequestNotFound", id].Value });

        var resource = RentalRequestResourceFromEntityAssembler.ToResourceFromEntity(rentalRequest);
        return Ok(resource);
    }

    /// <summary>
    ///     Gets all rental requests belonging to a specific client.
    /// </summary>
    /// <param name="clientId">Client identifier.</param>
    /// <returns>List of rental requests created by the client.</returns>
    /// <response code="200">Returns the list of rental requests for the client.</response>
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
    ///     Updates the status of an existing rental request.
    /// </summary>
    /// <remarks>
    ///     Business rules:
    ///     <list type="bullet">
    ///         <item><description>The caller must be authenticated.</description></item>
    ///         <item><description>Only users with role <c>Provider</c> can change the status.</description></item>
    ///         <item><description>Allowed statuses depend on the domain rules (for example: pending, approved, rejected, completed, cancelled).</description></item>
    ///     </list>
    ///
    ///     <para>Sample request body:</para>
    ///     <code language="json">
    ///     PUT /api/v1/RentalRequests/10
    ///     {
    ///       "status": "rejected"
    ///     }
    ///     </code>
    /// </remarks>
    /// <param name="id">Rental request identifier.</param>
    /// <param name="resource">New status data.</param>
    /// <returns>The rental request with the updated status.</returns>
    /// <response code="200">Status updated successfully.</response>
    /// <response code="400">Invalid status value.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User is authenticated but not a provider.</response>
    /// <response code="404">Rental request not found.</response>
    /// <response code="500">Unexpected error while updating the request.</response>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(RentalRequestResource), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateRentalRequestStatus(
        int id,
        [FromBody] UpdateRentalRequestStatusResource resource)
    {
        try
        {
            // Get authenticated user from middleware
            var authenticatedUser = HttpContext.Items["User"] as User;

            // Authorization: User must be authenticated
            if (authenticatedUser == null)
                return Unauthorized(new { message = localizer["AuthenticationRequired"].Value });

            // Authorization: Only Providers can update rental request status
            if (authenticatedUser.Role.ToRoleName() != "Provider")
                return StatusCode(StatusCodes.Status403Forbidden,
                    new { message = localizer["OnlyProvidersCanUpdateRentalRequestStatus"].Value });

            var command = new UpdateRentalRequestStatusCommand(id, resource.Status);
            var rentalRequest = await rentalRequestCommandService.Handle(command);

            if (rentalRequest == null)
                return NotFound(new { message = localizer["RentalRequestNotFound", id].Value });

            var rentalRequestResource = RentalRequestResourceFromEntityAssembler.ToResourceFromEntity(rentalRequest);
            return Ok(rentalRequestResource);
        }
        catch (ArgumentException)
        {
            return BadRequest(new { message = localizer["InvalidRentalStatus"].Value });
        }
        catch (Exception ex)
        {
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                new
                {
                    message = localizer["ErrorUpdatingRentalRequest"].Value,
                    detail = ex.Message
                });
        }
    }

    /// <summary>
    ///     Approves a rental request and automatically creates a billing invoice.
    /// </summary>
    /// <remarks>
    ///     Business rules:
    ///     <list type="bullet">
    ///         <item><description>The caller must be authenticated.</description></item>
    ///         <item><description>Only users with role <c>Provider</c> can approve requests.</description></item>
    ///         <item><description>Only requests in <c>pending</c> status can be approved.</description></item>
    ///         <item><description>On approval, an invoice is created for the client based on the monthly price.</description></item>
    ///     </list>
    /// </remarks>
    /// <param name="id">Rental request identifier.</param>
    /// <returns>The approved rental request with the associated invoice created.</returns>
    /// <response code="200">Request approved successfully and invoice created.</response>
    /// <response code="400">Invalid operation (for example, request not pending).</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User is authenticated but not a provider.</response>
    /// <response code="404">Rental request not found.</response>
    /// <response code="500">Unexpected error while approving the request.</response>
    [HttpPost("{id:int}/approve")]
    [ProducesResponseType(typeof(RentalRequestResource), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ApproveRentalRequest(int id)
    {
        try
        {
            // Get authenticated user from middleware
            var authenticatedUser = HttpContext.Items["User"] as User;

            // Authorization: User must be authenticated
            if (authenticatedUser == null)
                return Unauthorized(new { message = localizer["AuthenticationRequired"].Value });

            // Authorization: Only Providers can approve rental requests
            if (authenticatedUser.Role.ToRoleName() != "Provider")
                return StatusCode(StatusCodes.Status403Forbidden,
                    new { message = localizer["OnlyProvidersCanApproveRentalRequests"].Value });

            var command = new ApproveRentalRequestCommand(id, authenticatedUser.Id);
            var rentalRequest = await rentalRequestCommandService.Handle(command);

            if (rentalRequest == null)
                return NotFound(new { message = localizer["RentalRequestNotFound", id].Value });

            var rentalRequestResource = RentalRequestResourceFromEntityAssembler.ToResourceFromEntity(rentalRequest);
            return Ok(rentalRequestResource);
        }
        catch (InvalidOperationException ex)
        {
            var message = ex.Message == "Only pending rental requests can be approved"
                ? localizer["OnlyPendingRequestsCanBeApproved"].Value
                : localizer["GenericRentalOperationFailure"].Value;

            return BadRequest(new { message });
        }
        catch (Exception ex)
        {
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                new
                {
                    message = localizer["ErrorApprovingRentalRequest"].Value,
                    detail = ex.Message
                });
        }
    }
}
