using System.Net.Mime;
using coolgym_webapi.Contexts.maintenance.Domain.Commands;
using coolgym_webapi.Contexts.maintenance.Domain.Exceptions;
using coolgym_webapi.Contexts.maintenance.Domain.Queries;
using coolgym_webapi.Contexts.maintenance.Domain.Services;
using coolgym_webapi.Contexts.maintenance.Interfaces.REST.Resources;
using coolgym_webapi.Contexts.maintenance.Interfaces.REST.Transform;
using Microsoft.AspNetCore.Mvc;
using InvalidDataException = System.IO.InvalidDataException;

namespace coolgym_webapi.Contexts.maintenance.Interfaces.REST;

/// <summary>
///     Controller REST para gestionar solicitudes de mantenimiento (Maintenance Requests)
///     asociadas a los equipos de fitness del gimnasio.
/// </summary>
/// <remarks>
///     Permite registrar, consultar, filtrar, actualizar el estado y eliminar
///     solicitudes de mantenimiento. Estas solicitudes sirven para coordinar
///     el trabajo del personal técnico y mantener un historial de fallas y reparaciones.
/// </remarks>
[ApiController]
[Route("api/v1/[controller]")]
[Produces(MediaTypeNames.Application.Json)]
public class MaintenanceRequestsController(
    IMaintenanceRequestCommandService maintenanceRequestCommandService,
    IMaintenanceRequestQueryService maintenanceRequestQueryService) : ControllerBase
{
    /// <summary>
    ///     Registra una nueva solicitud de mantenimiento para un equipo.
    /// </summary>
    /// <remarks>
    ///     Crea una Maintenance Request asociada a un equipo determinado, incluyendo
    ///     información como el motivo de la falla y fecha de creación.
    ///     Ejemplo de solicitud:
    ///     POST /api/v1/maintenancerequests
    ///     {
    ///     "equipmentId": 1,
    ///     "selectedDate": "2025-01-15T10:30:00Z",
    ///     "observation": "Al correr a más de 8 km/h se escucha un golpe metálico."
    ///     }
    /// </remarks>
    /// <param name="resource">Datos de la solicitud de mantenimiento a registrar.</param>
    /// <returns>Solicitud de mantenimiento creada con su ID asignado automáticamente.</returns>
    /// <response code="201">Solicitud creada exitosamente. Retorna la solicitud completa con su ID.</response>
    /// <response code="400">Datos de entrada inválidos (validaciones de Value Objects o formato).</response>
    /// <response code="409">Conflicto: ya existe una solicitud pendiente similar para el mismo equipo.</response>
    /// <response code="500">Error interno del servidor.</response>
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
            var maintenanceRequestResource =
                MaintenanceRequestResourceFromEntityAssembler.ToResourceFromEntity(maintenanceRequest);

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


    /// <summary>
    ///     Obtiene todas las solicitudes de mantenimiento registradas.
    /// </summary>
    /// <remarks>
    ///     Retorna la lista completa de Maintenance Requests, incluyendo solicitudes
    ///     pendientes o completadas.
    ///     Ejemplo de solicitud:
    ///     GET /api/v1/maintenancerequests
    /// </remarks>
    /// <returns>Lista de todas las solicitudes de mantenimiento registradas.</returns>
    /// <response code="200">Retorna la lista de solicitudes exitosamente (puede ser vacía).</response>
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
    ///     Obtiene una solicitud de mantenimiento específica por su identificador único.
    /// </summary>
    /// <remarks>
    ///     Retorna los detalles de una Maintenance Request, incluyendo:
    ///     - Equipo asociado
    ///     - Fecha seleccionada para mantenimiento
    ///     - Descripción del problema
    ///     - Estado actual
    ///     Ejemplo de solicitud:
    ///     GET /api/v1/maintenancerequests/1
    /// </remarks>
    /// <param name="id">Identificador único de la solicitud de mantenimiento.</param>
    /// <returns>Solicitud de mantenimiento encontrada.</returns>
    /// <response code="200">Solicitud encontrada exitosamente.</response>
    /// <response code="404">No se encontró una solicitud con el ID especificado.</response>
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


    /// <summary>
    ///     Obtiene solicitudes de mantenimiento filtradas por su estado actual.
    /// </summary>
    /// <remarks>
    ///     Filtra las Maintenance Requests según su estado. Ejemplos de estados:
    ///     - <c>pending</c>: solicitud creada, en espera de atención
    ///     - <c>completed</c>: el mantenimiento fue completado
    ///     Ejemplo de solicitud:
    ///     GET /api/v1/maintenancerequests/status/pending
    /// </remarks>
    /// <param name="status">Estado de la solicitud (p. ej. "pending", "completed").</param>
    /// <returns>Lista de solicitudes de mantenimiento con el estado especificado.</returns>
    /// <response code="200">Retorna la lista de solicitudes filtradas (puede ser vacía).</response>
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
    ///     Actualiza el estado de una solicitud de mantenimiento existente.
    /// </summary>
    /// <remarks>
    ///     Permite cambiar el estado de la Maintenance Request (de
    ///     <c>pending</c> a <c>completed</c>).
    ///     Ejemplo de solicitud:
    ///     PUT /api/v1/maintenancerequests/1
    ///     {
    ///     "status": "completed"
    ///     }
    /// </remarks>
    /// <param name="id">Identificador de la solicitud de mantenimiento a actualizar.</param>
    /// <param name="resource">Datos con el nuevo estado y/o notas de la solicitud.</param>
    /// <returns>Solicitud de mantenimiento actualizada con los nuevos valores.</returns>
    /// <response code="200">Solicitud actualizada exitosamente.</response>
    /// <response code="400">Datos de entrada inválidos.</response>
    /// <response code="404">No se encontró una solicitud con el ID especificado.</response>
    /// <response code="500">Error interno del servidor.</response>
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
                return NotFound(new { message = $"Maintenance Request with id {id} not found" });

            var maintenanceRequestResource =
                MaintenanceRequestResourceFromEntityAssembler.ToResourceFromEntity(maintenanceRequest);
            return Ok(maintenanceRequestResource);
        }
        catch (MaintenanceRequestNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
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
                new { message = "An error occurred while updating the equipment", detail = ex.Message });
        }
    }

    /// <summary>
    ///     Elimina una solicitud de mantenimiento del sistema.
    /// </summary>
    /// <remarks>
    ///     Esta operación puede eliminar una solicitud de mantenimiento físicamente de la base de datos.
    ///     Si la solicitud no existe, se retorna un error 404.
    ///     Ejemplo de solicitud:
    ///     DELETE /api/v1/maintenancerequests/1
    /// </remarks>
    /// <param name="id">Identificador de la solicitud de mantenimiento a eliminar.</param>
    /// <returns><c>204 No Content</c> si la eliminación fue exitosa.</returns>
    /// <response code="204">Solicitud eliminada exitosamente (sin contenido en la respuesta).</response>
    /// <response code="404">No se encontró una solicitud con el ID especificado.</response>
    /// <response code="400">Datos inválidos para la eliminación.</response>
    /// <response code="500">Error interno del servidor.</response>
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
                return NotFound(new { message = $"Maintenance Request with id {id} not found" });

            return NoContent();
        }
        catch (MaintenanceRequestNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500,
                new { message = "An error occurred while deleting the equipment", detail = ex.Message });
        }
    }
}