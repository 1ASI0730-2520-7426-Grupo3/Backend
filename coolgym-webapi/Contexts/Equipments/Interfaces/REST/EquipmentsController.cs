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
///     Controller REST para gestionar Equipment
///     Expone endpoints HTTP para CRUD completo
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Produces(MediaTypeNames.Application.Json)]
public class EquipmentsController : ControllerBase
{
    private readonly IEquipmentCommandService _equipmentCommandService;
    private readonly IEquipmentQueryService _equipmentQueryService;

    public EquipmentsController(
        IEquipmentCommandService equipmentCommandService,
        IEquipmentQueryService equipmentQueryService)
    {
        _equipmentCommandService = equipmentCommandService;
        _equipmentQueryService = equipmentQueryService;
    }

    /// <summary>
    ///     GET /api/v1/equipments
    ///     Obtiene todos los equipos registrados
    /// </summary>
    /// <returns>Lista de todos los equipos</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<EquipmentResource>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllEquipments()
    {
        var query = new GetAllEquipment();
        var equipments = await _equipmentQueryService.Handle(query);
        var resources = EquipmentResourceFromEntityAssembler.ToResourceFromEntity(equipments);
        return Ok(resources);
    }

    /// <summary>
    ///     GET /api/v1/equipments/{id}
    ///     Obtiene un equipo específico por su ID
    /// </summary>
    /// <param name="id">ID del equipo</param>
    /// <returns>Equipo encontrado</returns>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(EquipmentResource), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetEquipmentById(int id)
    {
        var query = new GetEquipmentById(id);
        var equipment = await _equipmentQueryService.Handle(query);

        if (equipment == null)
            return NotFound(new { message = $"Equipment with id {id} not found" });

        var resource = EquipmentResourceFromEntityAssembler.ToResourceFromEntity(equipment);
        return Ok(resource);
    }

    /// <summary>
    ///     GET /api/v1/equipments/type/{type}
    ///     Obtiene equipos filtrados por tipo (ej: "Treadmill", "Bike")
    /// </summary>
    /// <param name="type">Tipo de equipo</param>
    /// <returns>Lista de equipos del tipo especificado</returns>
    [HttpGet("type/{type}")]
    [ProducesResponseType(typeof(IEnumerable<EquipmentResource>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetEquipmentsByType(string type)
    {
        var query = new GetEquipmentByType(type);
        var equipments = await _equipmentQueryService.Handle(query);
        var resources = EquipmentResourceFromEntityAssembler.ToResourceFromEntity(equipments);
        return Ok(resources);
    }

    /// <summary>
    ///     GET /api/v1/equipments/status/{status}
    ///     Obtiene equipos filtrados por estado (ej: "active", "maintenance")
    /// </summary>
    /// <param name="status">Estado del equipo</param>
    /// <returns>Lista de equipos con el estado especificado</returns>
    [HttpGet("status/{status}")]
    [ProducesResponseType(typeof(IEnumerable<EquipmentResource>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetEquipmentsByStatus(string status)
    {
        var query = new GetEquipmentByStatus(status);
        var equipments = await _equipmentQueryService.Handle(query);
        var resources = EquipmentResourceFromEntityAssembler.ToResourceFromEntity(equipments);
        return Ok(resources);
    }

    /// <summary>
    ///     POST /api/v1/equipments
    ///     Crea un nuevo equipo
    /// </summary>
    /// <param name="resource">Datos del equipo a crear</param>
    /// <returns>Equipo creado con su ID asignado</returns>
    [HttpPost]
    [ProducesResponseType(typeof(EquipmentResource), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateEquipment([FromBody] CreateEquipmentResource resource)
    {
        try
        {
            // Convertir Resource → Command
            var command = CreateEquipmentCommandFromResourceAssembler.ToCommandFromResource(resource);
            // Ejecutar el comando
            var equipment = await _equipmentCommandService.Handle(command);
            // Convertir Entity → Resource
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
    ///     PUT /api/v1/equipments/{id}
    ///     Actualiza un equipo existente
    /// </summary>
    /// <param name="id">ID del equipo a actualizar</param>
    /// <param name="resource">Nuevos datos del equipo</param>
    /// <returns>Equipo actualizado</returns>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(EquipmentResource), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateEquipment(int id, [FromBody] UpdateEquipmentResource resource)
    {
        try
        {
            // Convertir Resource → Command
            var command = UpdateEquipmentCommandFromResourceAssembler.ToCommandFromResource(id, resource);

            // Ejecutar el comando
            var equipment = await _equipmentCommandService.Handle(command);

            // Verificar si se encontró el equipo
            if (equipment == null)
                return NotFound(new { message = $"Equipment with id {id} not found" });

            // Convertir Entity → Resource
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
    ///     DELETE /api/v1/equipments/{id}
    ///     Elimina un equipo
    /// </summary>
    /// <param name="id">ID del equipo a eliminar</param>
    /// <returns>204 No Content si se eliminó correctamente</returns>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> DeleteEquipment(int id)
    {
        try
        {
            var command = new DeleteEquipmentCommand(id);
            var result = await _equipmentCommandService.Handle(command);

            if (!result)
                return NotFound(new { message = $"Equipment with id {id} not found" });

            // 204 No Content: Éxito sin cuerpo de respuesta
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