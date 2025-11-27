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
///     Controller REST para gestionar Equipment (equipos de fitness)
///     Expone endpoints HTTP para operaciones CRUD completas con monitoreo en tiempo real
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Produces(MediaTypeNames.Application.Json)]
public class EquipmentsController(
    IEquipmentCommandService equipmentCommandService,
    IEquipmentQueryService equipmentQueryService) : ControllerBase
{
    /// <summary>
    ///     Obtiene todos los equipos de fitness registrados en el sistema
    /// </summary>
    /// <remarks>
    ///     Retorna una lista completa de equipos activos (no eliminados lógicamente).
    ///     Cada equipo incluye información de ubicación, uso, controles y mantenimiento.
    ///     Ejemplo de respuesta:
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
    /// <returns>Lista de todos los equipos registrados</returns>
    /// <response code="200">Retorna la lista de equipos exitosamente</response>
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
    ///     Obtiene un equipo específico por su identificador único
    /// </summary>
    /// <remarks>
    ///     Retorna la información detallada de un equipo incluyendo:
    ///     - Datos básicos (nombre, tipo, modelo, fabricante)
    ///     - Ubicación actual
    ///     - Estadísticas de uso en tiempo real
    ///     - Configuración de controles
    ///     - Historial de mantenimiento
    ///     Ejemplo de solicitud:
    ///     GET /api/v1/equipments/1
    /// </remarks>
    /// <param name="id">Identificador único del equipo (número entero positivo)</param>
    /// <returns>Equipo encontrado con toda su información</returns>
    /// <response code="200">Equipo encontrado exitosamente</response>
    /// <response code="404">No se encontró un equipo con el ID especificado</response>
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
    ///     Obtiene equipos filtrados por tipo (categoría)
    /// </summary>
    /// <remarks>
    ///     Filtra equipos según su categoría. Tipos válidos incluyen:
    ///     - **treadmill**: Caminadoras y cintas de correr
    ///     - **bike**: Bicicletas estáticas
    ///     - **elliptical**: Máquinas elípticas
    ///     - **rower**: Máquinas de remo
    ///     Ejemplo de solicitud:
    ///     GET /api/v1/equipments/type/treadmill
    /// </remarks>
    /// <param name="type">Tipo/categoría del equipo (ej: "treadmill", "bike", "elliptical")</param>
    /// <returns>Lista de equipos del tipo especificado</returns>
    /// <response code="200">Retorna lista de equipos filtrados (puede estar vacía)</response>
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
    ///     Obtiene equipos filtrados por su estado operativo
    /// </summary>
    /// <remarks>
    ///     Filtra equipos según su estado actual. Estados válidos:
    ///     - **active**: Equipo operativo y disponible para uso
    ///     - **maintenance**: Equipo en mantenimiento programado
    ///     - **pending_maintenance**: Requiere mantenimiento pronto
    ///     - **inactive**: Equipo temporalmente fuera de servicio
    ///     Ejemplo de solicitud:
    ///     GET /api/v1/equipments/status/active
    /// </remarks>
    /// <param name="status">Estado del equipo (ej: "active", "maintenance", "inactive")</param>
    /// <returns>Lista de equipos con el estado especificado</returns>
    /// <response code="200">Retorna lista de equipos filtrados</response>
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
    ///     Registra un nuevo equipo de fitness en el sistema
    /// </summary>
    /// <remarks>
    ///     Crea un nuevo equipo con validaciones de negocio:
    ///     - El número de serie debe ser único en el sistema
    ///     - La ubicación debe tener nombre y dirección válidos
    ///     - Los datos técnicos (potencia, modelo) son obligatorios
    ///     Ejemplo de solicitud:
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
    /// <param name="resource">Datos del equipo a registrar</param>
    /// <returns>Equipo creado con su ID asignado automáticamente</returns>
    /// <response code="201">Equipo creado exitosamente. Retorna el equipo completo con su ID</response>
    /// <response code="400">Datos de entrada inválidos (validaciones de Value Objects)</response>
    /// <response code="409">Conflicto: Ya existe un equipo con ese número de serie</response>
    /// <response code="500">Error interno del servidor</response>
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
    ///     Actualiza la información de un equipo existente
    /// </summary>
    /// <remarks>
    ///     Permite modificar datos de un equipo previamente registrado.
    ///     **Campos actualizables:**
    ///     - Nombre y código del equipo
    ///     - Potencia eléctrica (watts)
    ///     - Estado de encendido (on/off)
    ///     - Estado operativo
    ///     - Ubicación (nombre y dirección)
    ///     - Notas adicionales
    ///     - Imagen/foto del equipo
    ///     **Campos NO modificables:**
    ///     - Número de serie (inmutable por seguridad)
    ///     - Fecha de instalación
    ///     - Modelo y fabricante
    ///     Ejemplo de solicitud:
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
    /// <param name="id">Identificador del equipo a actualizar</param>
    /// <param name="resource">Nuevos datos del equipo</param>
    /// <returns>Equipo actualizado con los nuevos valores</returns>
    /// <response code="200">Equipo actualizado exitosamente</response>
    /// <response code="400">Datos de entrada inválidos</response>
    /// <response code="404">No se encontró un equipo con el ID especificado</response>
    /// <response code="500">Error interno del servidor</response>
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
    ///     Elimina (desactiva) un equipo del sistema mediante borrado lógico
    /// </summary>
    /// <remarks>
    ///     **Importante:** Esta operación NO borra físicamente el equipo de la base de datos.
    ///     Realiza un "soft delete" marcando el campo `IsDeleted = 1`.
    ///     **Validaciones de negocio antes de eliminar:**
    ///     - ❌ El equipo NO debe estar encendido (IsPoweredOn = true)
    ///     - ❌ El equipo NO debe estar en mantenimiento (Status = "maintenance")
    ///     Si alguna validación falla, la operación se rechaza con error 400.
    ///     **Recuperación:**
    ///     Los equipos eliminados lógicamente pueden recuperarse mediante
    ///     un script de base de datos cambiando `IsDeleted = 0`.
    ///     Ejemplo de solicitud:
    ///     DELETE /api/v1/equipments/1
    /// </remarks>
    /// <param name="id">Identificador del equipo a eliminar</param>
    /// <returns>204 No Content si se eliminó correctamente</returns>
    /// <response code="204">Equipo eliminado exitosamente (sin contenido en respuesta)</response>
    /// <response code="400">No se puede eliminar: equipo encendido o en mantenimiento</response>
    /// <response code="404">No se encontró un equipo con el ID especificado</response>
    /// <response code="500">Error interno del servidor</response>
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