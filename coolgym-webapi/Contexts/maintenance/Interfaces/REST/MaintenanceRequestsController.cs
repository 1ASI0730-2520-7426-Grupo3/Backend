using System.Net.Mime;
using coolgym_webapi.Contexts.Equipments.Domain.Exceptions;
using coolgym_webapi.Contexts.maintenance.Domain.Commands;
using coolgym_webapi.Contexts.maintenance.Domain.Exceptions;
using coolgym_webapi.Contexts.maintenance.Domain.Queries;
using coolgym_webapi.Contexts.maintenance.Domain.Services;
using coolgym_webapi.Contexts.maintenance.Interfaces.REST.Resources;
using coolgym_webapi.Contexts.maintenance.Interfaces.REST.Transform;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using InvalidDataException = coolgym_webapi.Contexts.maintenance.Domain.Exceptions.InvalidDataException;

namespace coolgym_webapi.Contexts.maintenance.Interfaces.REST;

/// <summary>
///     REST controller that exposes endpoints to manage maintenance requests
///     for CoolGym equipments.
/// </summary>
/// <remarks>
///     All error messages returned by this controller are localized according
///     to the <c>Accept-Language</c> HTTP header (supports <c>en</c> and <c>es</c>).
/// </remarks>
[ApiController]
[Route("api/v1/[controller]")]
[Produces(MediaTypeNames.Application.Json)]
public class MaintenanceRequestsController(
    IMaintenanceRequestCommandService maintenanceRequestCommandService,
    IMaintenanceRequestQueryService maintenanceRequestQueryService,
    IStringLocalizer<MaintenanceRequestsController> localizer) : ControllerBase
{
    /// <summary>
    ///     Registers a new maintenance request for an equipment.
    /// </summary>
    /// <param name="resource">Maintenance request data.</param>
    /// <returns>
    ///     Returns <c>201 Created</c> with the created maintenance request when the operation succeeds.
    ///     Returns <c>400 Bad Request</c> when the provided data is invalid.
    ///     Returns <c>404 Not Found</c> when the referenced equipment does not exist.
    ///     Returns <c>409 Conflict</c> when a similar maintenance request already exists.
    ///     Returns <c>500 Internal Server Error</c> when an unexpected error occurs.
    /// </returns>
    /// <remarks>
    ///     Typical usage is when a gym administrator or automated monitoring system
    ///     detects a problem in an equipment and needs to schedule a maintenance visit.
    ///     <para>Sample request body:</para>
    ///     <code language="json">
    /// {
    ///   "equipmentId": 1243234325,
    ///   "selectedDate": "2025-12-21T10:30:00Z",
    ///   "observation": "Replace belts and lubricate the running surface."
    /// }
    /// </code>
    ///     <para>Business rules applied:</para>
    ///     <list type="bullet">
    ///         <item>The equipment must exist in the system.</item>
    ///         <item>A duplicate maintenance request for the same equipment and date is not allowed.</item>
    ///         <item>Domain validations may reject inconsistent or incomplete data.</item>
    ///     </list>
    ///     <para>
    ///         Error messages are localized based on the <c>Accept-Language</c> header.
    ///     </para>
    /// </remarks>
    [HttpPost]
    [ProducesResponseType(typeof(MaintenanceRequestResource), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateMaintenanceRequest([FromBody] CreateMaintenanceRequestResource resource)
    {
        try
        {
            var command = CreateMaintenanceRequestCommandFromResourceAssembler.ToCommandFromResource(resource);
            var maintenanceRequest = await maintenanceRequestCommandService.Handle(command);
            var maintenanceRequestResource =
                MaintenanceRequestResourceFromEntityAssembler.ToResourceFromEntity(maintenanceRequest);

            return CreatedAtAction(
                nameof(GetMaintenanceRequestById),
                new { id = maintenanceRequest.Id },
                maintenanceRequestResource);
        }
        catch (DuplicateEquipmentMaintenanceRequestException ex)
        {
            return Conflict(new { message = localizer[ex.Message].Value });
        }
        catch (EquipmentNotFoundException ex)
        {
            return NotFound(new { message = localizer[ex.Message].Value });
        }
        catch (InvalidDataException ex)
        {
            return BadRequest(new { message = localizer[ex.Message].Value });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = localizer[ex.Message].Value });
        }
        catch (Exception ex)
        {
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                new
                {
                    message = localizer["An error occurred while creating the maintenance request."].Value,
                    detail = ex.Message
                });
        }
    }

    /// <summary>
    ///     Gets all maintenance requests.
    /// </summary>
    /// <returns>
    ///     Returns <c>200 OK</c> with the complete list of maintenance requests.
    /// </returns>
    /// <remarks>
    ///     This endpoint is typically used by back-office dashboards to display
    ///     all open and historical maintenance requests.
    /// </remarks>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<MaintenanceRequestResource>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllMaintenanceRequests()
    {
        var query = new GetAllMaintenanceRequests();
        var maintenanceRequests = await maintenanceRequestQueryService.Handle(query);
        var resources = MaintenanceRequestResourceFromEntityAssembler.ToResourceFromEntity(maintenanceRequests);
        return Ok(resources);
    }

    /// <summary>
    ///     Gets a maintenance request by its identifier.
    /// </summary>
    /// <param name="id">Maintenance request identifier.</param>
    /// <returns>
    ///     Returns <c>200 OK</c> with the maintenance request when it exists.
    ///     Returns <c>404 Not Found</c> when the maintenance request cannot be found.
    /// </returns>
    /// <remarks>
    ///     Use this endpoint to show detailed information about a single maintenance
    ///     request in the UI or when debugging specific cases.
    /// </remarks>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(MaintenanceRequestResource), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetMaintenanceRequestById(int id)
    {
        var query = new GetMaintenanceRequestById(id);
        var maintenanceRequest = await maintenanceRequestQueryService.Handle(query);
        if (maintenanceRequest == null)
            return NotFound(new { message = localizer[$"Maintenance request with id {id} was not found."].Value });

        var resource = MaintenanceRequestResourceFromEntityAssembler.ToResourceFromEntity(maintenanceRequest);
        return Ok(resource);
    }

    /// <summary>
    ///     Gets maintenance requests filtered by status.
    /// </summary>
    /// <param name="status">
    ///     Maintenance request status (for example, <c>pending</c>, <c>in-progress</c>, <c>completed</c>).
    /// </param>
    /// <returns>
    ///     Returns <c>200 OK</c> with the list of maintenance requests that match the given status.
    /// </returns>
    /// <remarks>
    ///     This endpoint is useful to build filtered views, such as "Pending maintenance"
    ///     or "Completed maintenance" dashboards.
    /// </remarks>
    [HttpGet("status/{status}")]
    [ProducesResponseType(typeof(IEnumerable<MaintenanceRequestResource>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMaintenanceRequestsByStatus(string status)
    {
        var query = new GetMaintenanceRequestsByStatus(status);
        var maintenanceRequests = await maintenanceRequestQueryService.Handle(query);
        var resources = MaintenanceRequestResourceFromEntityAssembler.ToResourceFromEntity(maintenanceRequests);
        return Ok(resources);
    }

    /// <summary>
    ///     Updates the status of an existing maintenance request.
    /// </summary>
    /// <param name="id">Maintenance request identifier.</param>
    /// <param name="resource">Data that contains the new status.</param>
    /// <returns>
    ///     Returns <c>200 OK</c> with the updated maintenance request when the operation succeeds.
    ///     Returns <c>404 Not Found</c> when the maintenance request cannot be found.
    ///     Returns <c>400 Bad Request</c> when the new status is not valid or violates domain rules.
    ///     Returns <c>500 Internal Server Error</c> when an unexpected error occurs.
    /// </returns>
    /// <remarks>
    ///     This endpoint is commonly used by maintenance staff to move requests through
    ///     their lifecycle (for example, from <c>pending</c> to <c>in-progress</c> and
    ///     finally to <c>completed</c>).
    /// </remarks>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(MaintenanceRequestResource), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateMaintenanceRequestStatus(int id,
        [FromBody] UpdateMaintenanceRequestStatusResource resource)
    {
        try
        {
            var command =
                UpdateMaintenanceRequestStatusCommandFromResourceAssembler.ToCommandFromResource(id, resource);
            var maintenanceRequest = await maintenanceRequestCommandService.Handle(command);
            if (maintenanceRequest == null)
                return NotFound(new { message = localizer[$"Maintenance request with id {id} was not found."].Value });

            var maintenanceRequestResource =
                MaintenanceRequestResourceFromEntityAssembler.ToResourceFromEntity(maintenanceRequest);
            return Ok(maintenanceRequestResource);
        }
        catch (MaintenanceRequestNotFoundException ex)
        {
            return NotFound(new { message = localizer[ex.Message].Value });
        }
        catch (InvalidDataException ex)
        {
            return BadRequest(new { message = localizer[ex.Message].Value });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = localizer[ex.Message].Value });
        }
        catch (Exception ex)
        {
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                new
                {
                    message = localizer["An error occurred while updating the maintenance request."].Value,
                    detail = ex.Message
                });
        }
    }

    /// <summary>
    ///     Deletes a maintenance request.
    /// </summary>
    /// <param name="id">Maintenance request identifier.</param>
    /// <returns>
    ///     Returns <c>204 No Content</c> when the maintenance request is successfully deleted.
    ///     Returns <c>404 Not Found</c> when no maintenance request exists with the provided identifier.
    ///     Returns <c>400 Bad Request</c> or <c>500 Internal Server Error</c> in case of failures.
    /// </returns>
    /// <remarks>
    ///     Deleting maintenance requests should be done with caution and is usually reserved
    ///     for administrative tasks, such as removing test data or correcting erroneous records.
    /// </remarks>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteMaintenanceRequest(int id)
    {
        try
        {
            var command = new DeleteMaintenanceRequestCommand(id);
            var result = await maintenanceRequestCommandService.Handle(command);
            if (!result)
                return NotFound(new { message = localizer[$"Maintenance request with id {id} was not found."].Value });

            return NoContent();
        }
        catch (MaintenanceRequestNotFoundException ex)
        {
            return NotFound(new { message = localizer[ex.Message].Value });
        }
        catch (Exception ex)
        {
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                new
                {
                    message = localizer["An error occurred while deleting the maintenance request."].Value,
                    detail = ex.Message
                });
        }
    }
}