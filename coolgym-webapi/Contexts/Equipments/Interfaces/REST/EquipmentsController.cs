using System.Net.Mime;
using coolgym_webapi.Contexts.Equipments.Domain.Commands;
using coolgym_webapi.Contexts.Equipments.Domain.Exceptions;
using coolgym_webapi.Contexts.Equipments.Domain.Queries;
using coolgym_webapi.Contexts.Equipments.Domain.Services;
using coolgym_webapi.Contexts.Equipments.Interfaces.REST.Resources;
using coolgym_webapi.Contexts.Equipments.Interfaces.REST.Transform;
using Microsoft.AspNetCore.Mvc;

namespace coolgym_webapi.Contexts.Equipments.Interfaces.REST;

/// <summary>
///     REST API Controller for managing fitness equipment
///     Provides HTTP endpoints for complete CRUD operations with real-time monitoring
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Produces(MediaTypeNames.Application.Json)]
public class EquipmentsController(
    IEquipmentCommandService equipmentCommandService,
    IEquipmentQueryService equipmentQueryService) : ControllerBase
{
    /// <summary>
    ///     Gets all fitness equipment registered in the system
    /// </summary>
    /// <remarks>
    ///     Returns a complete list of active equipment (not logically deleted).
    ///     Each equipment includes location, usage, control, and maintenance information.
    ///     Example response:
    ///     GET /api/v1/equipments
    ///     [
    ///     {
    ///     "id": 1,
    ///     "name": "Treadmill Pro X-500",
    ///     "type": "treadmill",
    ///     "status": "active",
    ///     "isPoweredOn": true,
    ///     "location": {
    ///     "name": "Cardio Area",
    ///     "address": "Floor 1, Section A"
    ///     }
    ///     }
    ///     ]
    /// </remarks>
    /// <returns>List of all registered equipment</returns>
    /// <response code="200">Returns the equipment list successfully</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<EquipmentResource>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllEquipments()
    {
        var query = new GetAllEquipment();
        var equipments = await equipmentQueryService.Handle(query);
        var resources = EquipmentResourceFromEntityAssembler.ToResourceFromEntity(equipments);
        return Ok(resources);
    }

    /// <summary>
    ///     Gets a specific equipment by its unique identifier
    /// </summary>
    /// <remarks>
    ///     Returns detailed information about an equipment including:
    ///     - Basic data (name, type, model, manufacturer)
    ///     - Current location
    ///     - Real-time usage statistics
    ///     - Control configuration
    ///     - Maintenance history
    ///     Example request:
    ///     GET /api/v1/equipments/1
    /// </remarks>
    /// <param name="id">Unique equipment identifier (positive integer)</param>
    /// <returns>Found equipment with all its information</returns>
    /// <response code="200">Equipment found successfully</response>
    /// <response code="404">Equipment with specified ID not found</response>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(EquipmentResource), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetEquipmentById(int id)
    {
        var query = new GetEquipmentById(id);
        var equipment = await equipmentQueryService.Handle(query);

        if (equipment == null)
            return NotFound(new { message = $"Equipment with id {id} not found" });

        var resource = EquipmentResourceFromEntityAssembler.ToResourceFromEntity(equipment);
        return Ok(resource);
    }

    /// <summary>
    ///     Gets equipment filtered by type (category)
    /// </summary>
    /// <remarks>
    ///     Filters equipment by category. Valid types include:
    ///     - **treadmill**: Treadmills and running machines
    ///     - **bike**: Stationary bikes
    ///     - **elliptical**: Elliptical machines
    ///     - **rower**: Rowing machines
    ///     Example request:
    ///     GET /api/v1/equipments/type/treadmill
    /// </remarks>
    /// <param name="type">Equipment type/category (e.g., "treadmill", "bike", "elliptical")</param>
    /// <returns>List of equipment of the specified type</returns>
    /// <response code="200">Returns filtered equipment list (may be empty)</response>
    [HttpGet("type/{type}")]
    [ProducesResponseType(typeof(IEnumerable<EquipmentResource>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetEquipmentsByType(string type)
    {
        var query = new GetEquipmentByType(type);
        var equipments = await equipmentQueryService.Handle(query);
        var resources = EquipmentResourceFromEntityAssembler.ToResourceFromEntity(equipments);
        return Ok(resources);
    }

    /// <summary>
    ///     Gets equipment filtered by operational status
    /// </summary>
    /// <remarks>
    ///     Filters equipment by current status. Valid statuses:
    ///     - **active**: Equipment operational and available for use
    ///     - **maintenance**: Equipment under scheduled maintenance
    ///     - **pending_maintenance**: Requires maintenance soon
    ///     - **inactive**: Equipment temporarily out of service
    ///     Example request:
    ///     GET /api/v1/equipments/status/active
    /// </remarks>
    /// <param name="status">Equipment status (e.g., "active", "maintenance", "inactive")</param>
    /// <returns>List of equipment with the specified status</returns>
    /// <response code="200">Returns filtered equipment list</response>
    [HttpGet("status/{status}")]
    [ProducesResponseType(typeof(IEnumerable<EquipmentResource>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetEquipmentsByStatus(string status)
    {
        var query = new GetEquipmentByStatus(status);
        var equipments = await equipmentQueryService.Handle(query);
        var resources = EquipmentResourceFromEntityAssembler.ToResourceFromEntity(equipments);
        return Ok(resources);
    }

    /// <summary>
    ///     Registers new fitness equipment in the system
    /// </summary>
    /// <remarks>
    ///     Creates new equipment with business rule validations:
    ///     - Serial number must be unique in the system
    ///     - Location must have valid name and address
    ///     - Technical data (power, model) are mandatory
    ///     Example request:
    ///     POST /api/v1/equipments
    ///     {
    ///     "name": "Treadmill Pro X-500",
    ///     "type": "treadmill",
    ///     "model": "TRX-500",
    ///     "manufacturer": "LifeFitness",
    ///     "serialNumber": "TRX500-001",
    ///     "code": "CAM-001",
    ///     "installationDate": "2025-01-15",
    ///     "powerWatts": 1500,
    ///     "locationName": "Cardio Area",
    ///     "locationAddress": "Floor 1, Section A",
    ///     "image": "https://example.com/images/treadmill.jpg"
    ///     }
    /// </remarks>
    /// <param name="resource">Equipment data to register</param>
    /// <returns>Created equipment with automatically assigned ID</returns>
    /// <response code="201">Equipment created successfully. Returns complete equipment with ID</response>
    /// <response code="400">Invalid input data (Value Object validations)</response>
    /// <response code="409">Conflict: Equipment with that serial number already exists</response>
    /// <response code="500">Internal server error</response>
    [HttpPost]
    [ProducesResponseType(typeof(EquipmentResource), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateEquipment([FromBody] CreateEquipmentResource resource)
    {
        try
        {
            var command = CreateEquipmentCommandFromResourceAssembler.ToCommandFromResource(resource);
            var equipment = await equipmentCommandService.Handle(command);
            var equipmentResource = EquipmentResourceFromEntityAssembler.ToResourceFromEntity(equipment);

            return CreatedAtAction(
                nameof(GetEquipmentById),
                new { id = equipment.Id },
                equipmentResource
            );
        }
        catch (DuplicateSerialNumberException ex)
        {
            return Conflict(new { message = ex.Message });
        }
        catch (InvalidLocationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidControlSettingsException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidUsageStatsException ex)
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
                new { message = "An error occurred while creating the equipment", detail = ex.Message });
        }
    }

    /// <summary>
    ///     Updates information of existing equipment
    /// </summary>
    /// <remarks>
    ///     Allows modifying data of previously registered equipment.
    ///     **Updatable fields:**
    ///     - Name and equipment code
    ///     - Electrical power (watts)
    ///     - Power status (on/off)
    ///     - Operational status
    ///     - Location (name and address)
    ///     - Additional notes
    ///     - Equipment image/photo
    ///     **NON-modifiable fields:**
    ///     - Serial number (immutable for security)
    ///     - Installation date
    ///     - Model and manufacturer
    ///     Example request:
    ///     PUT /api/v1/equipments/1
    ///     {
    ///     "name": "Treadmill Pro X-500 Updated",
    ///     "code": "CAM-001-MOD",
    ///     "powerWatts": 1600,
    ///     "isPoweredOn": true,
    ///     "activeStatus": "Normal",
    ///     "notes": "Recently serviced",
    ///     "status": "active",
    ///     "locationName": "Cardio Area VIP",
    ///     "locationAddress": "Floor 2, Section B",
    ///     "image": "https://example.com/images/treadmill-updated.jpg"
    ///     }
    /// </remarks>
    /// <param name="id">Equipment identifier to update</param>
    /// <param name="resource">New equipment data</param>
    /// <returns>Updated equipment with new values</returns>
    /// <response code="200">Equipment updated successfully</response>
    /// <response code="400">Invalid input data</response>
    /// <response code="404">Equipment with specified ID not found</response>
    /// <response code="500">Internal server error</response>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(EquipmentResource), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateEquipment(int id, [FromBody] UpdateEquipmentResource resource)
    {
        try
        {
            var command = UpdateEquipmentCommandFromResourceAssembler.ToCommandFromResource(id, resource);
            var equipment = await equipmentCommandService.Handle(command);

            if (equipment == null)
                return NotFound(new { message = $"Equipment with id {id} not found" });

            var equipmentResource = EquipmentResourceFromEntityAssembler.ToResourceFromEntity(equipment);
            return Ok(equipmentResource);
        }
        catch (EquipmentNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidStatusException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidLocationException ex)
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
                new { message = "An error occurred while updating the equipment", detail = ex.Message });
        }
    }

    /// <summary>
    ///     Deletes (deactivates) equipment from the system via soft delete
    /// </summary>
    /// <remarks>
    ///     **Important:** This operation does NOT physically delete equipment from the database.
    ///     Performs a "soft delete" by marking the `IsDeleted = 1` field.
    ///     **Business rule validations before deletion:**
    ///     - ❌ Equipment must NOT be powered on (IsPoweredOn = true)
    ///     - ❌ Equipment must NOT be under maintenance (Status = "maintenance")
    ///     If any validation fails, the operation is rejected with error 400.
    ///     **Recovery:**
    ///     Logically deleted equipment can be recovered via
    ///     a database script by changing `IsDeleted = 0`.
    ///     Example request:
    ///     DELETE /api/v1/equipments/1
    /// </remarks>
    /// <param name="id">Equipment identifier to delete</param>
    /// <returns>204 No Content if deleted successfully</returns>
    /// <response code="204">Equipment deleted successfully (no content in response)</response>
    /// <response code="400">Cannot delete: equipment is powered on or under maintenance</response>
    /// <response code="404">Equipment with specified ID not found</response>
    /// <response code="500">Internal server error</response>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteEquipment(int id)
    {
        try
        {
            var command = new DeleteEquipmentCommand(id);
            var result = await equipmentCommandService.Handle(command);

            if (!result)
                return NotFound(new { message = $"Equipment with id {id} not found" });

            return NoContent();
        }
        catch (EquipmentNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (EquipmentPoweredOnException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (EquipmentInMaintenanceException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500,
                new { message = "An error occurred while deleting the equipment", detail = ex.Message });
        }
    }
}