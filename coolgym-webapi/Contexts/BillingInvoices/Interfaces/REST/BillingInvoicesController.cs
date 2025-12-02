using System.Net.Mime;
using coolgym_webapi.Contexts.BillingInvoices.Domain.Commands;
using coolgym_webapi.Contexts.BillingInvoices.Domain.Queries;
using coolgym_webapi.Contexts.BillingInvoices.Domain.Services;
using coolgym_webapi.Contexts.BillingInvoices.Interfaces.REST.Resources;
using coolgym_webapi.Contexts.BillingInvoices.Interfaces.REST.Transform;
using coolgym_webapi.Contexts.Equipments.Domain.Queries;
using coolgym_webapi.Contexts.Equipments.Domain.Services;
using coolgym_webapi.Contexts.maintenance.Domain.Queries;
using coolgym_webapi.Contexts.maintenance.Domain.Services;
using coolgym_webapi.Contexts.Security.Domain.Model.Entities;
using coolgym_webapi.Contexts.Security.Domain.Model.ValueObjects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace coolgym_webapi.Contexts.BillingInvoices.Interfaces.REST;

/// <summary>
///     REST controller that exposes endpoints to query and manage billing invoices
///     associated with equipment maintenance operations.
/// </summary>
/// <remarks>
///     All error messages returned by this controller are localized according to the
///     <c>Accept-Language</c> HTTP header (currently supporting <c>en</c> and <c>es</c>).
/// </remarks>
[ApiController]
[Route("api/v1/billing/invoices")]
[Produces(MediaTypeNames.Application.Json)]
public class BillingInvoicesController(
    IInvoiceQueryService invoiceQueryService,
    IInvoiceCommandService invoiceCommandService,
    IMaintenanceRequestQueryService maintenanceRequestQueryService,
    IEquipmentQueryService equipmentQueryService,
    IStringLocalizer<BillingInvoicesController> localizer) : ControllerBase
{
    /// <summary>
    ///     Internal constant that represents the status of a maintenance request
    ///     that is eligible to generate an invoice.
    /// </summary>
    private const string CompletedStatus = "completed";

    /// <summary>
    ///     Gets all invoices for a specific user.
    /// </summary>
    /// <param name="userId">
    ///     Unique identifier of the user whose invoices are being requested.
    /// </param>
    /// <returns>
    ///     Returns <c>200 OK</c> with the list of invoices when the user identifier is valid.
    ///     Returns <c>400 Bad Request</c> when the user identifier is less than or equal to zero.
    /// </returns>
    /// <remarks>
    ///     This endpoint is typically used by the frontend "My invoices" view to show
    ///     all invoices that belong to the currently authenticated user.
    /// </remarks>
    [HttpGet]
    public async Task<IActionResult> GetInvoicesByUserId([FromQuery] int userId)
    {
        var authenticatedUser = HttpContext.Items["User"] as User;

        if (authenticatedUser == null)
            return Unauthorized(new { message = "Authentication required" });

        if (authenticatedUser.Role.ToRoleName() != "Client")
            return StatusCode(403, new { message = "Only clients can access billing invoices" });

        if (authenticatedUser.Id != userId)
            return StatusCode(403, new { message = "You can only access your own invoices" });

        if (userId <= 0) return BadRequest(new { message = localizer["User ID must be positive."].Value });

        var query = new GetInvoicesByUserId(userId);
        var invoices = await invoiceQueryService.Handle(query);
        var resources = invoices.Select(InvoiceResourceFromEntityAssembler.ToResourceFromEntity);

        return Ok(resources);
    }

    /// <summary>
    ///     Gets a specific invoice by its identifier.
    /// </summary>
    /// <param name="id">Unique identifier of the invoice.</param>
    /// <returns>
    ///     Returns <c>200 OK</c> with the invoice when it exists.
    ///     Returns <c>404 Not Found</c> when no invoice can be found with the given identifier.
    /// </returns>
    /// <remarks>
    ///     This endpoint is useful when the UI needs to display the full details
    ///     of a single invoice (for example, an invoice detail page or a PDF view).
    /// </remarks>
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetInvoiceById([FromRoute] int id)
    {
        var authenticatedUser = HttpContext.Items["User"] as User;

        if (authenticatedUser == null)
            return Unauthorized(new { message = "Authentication required" });

        if (authenticatedUser.Role.ToRoleName() != "Client")
            return StatusCode(403, new { message = "Only clients can access billing invoices" });

        var query = new GetInvoiceById(id);
        var invoice = await invoiceQueryService.Handle(query);
        if (invoice == null) return NotFound(new { message = localizer[$"Invoice with ID {id} was not found."].Value });

        if (authenticatedUser.Id != invoice.UserId)
            return StatusCode(403, new { message = "You can only access your own invoices" });

        var resource = InvoiceResourceFromEntityAssembler.ToResourceFromEntity(invoice);
        return Ok(resource);
    }

    /// <summary>
    ///     Gets all invoices stored in the system.
    /// </summary>
    /// <returns>
    ///     Returns <c>200 OK</c> with the complete collection of invoices.
    /// </returns>
    /// <remarks>
    ///     This endpoint is intended mainly for administrative purposes (for example,
    ///     back-office dashboards or reporting tools) and should not usually be exposed
    ///     directly to regular end users.
    /// </remarks>
    [HttpGet("all")]
    public async Task<IActionResult> GetAllInvoices()
    {
        var query = new GetAllInvoices();
        var invoices = await invoiceQueryService.Handle(query);
        var resources = invoices.Select(InvoiceResourceFromEntityAssembler.ToResourceFromEntity);
        return Ok(resources);
    }

    /// <summary>
    ///     Creates a new billing invoice.
    /// </summary>
    /// <param name="resource">
    ///     Data required to create the invoice, including optional linkage to a maintenance request.
    /// </param>
    /// <returns>
    ///     Returns <c>201 Created</c> with the created invoice when the operation succeeds.
    ///     Returns <c>400 Bad Request</c> when domain rules are violated (for example,
    ///     invalid amounts or dates).
    ///     Returns <c>404 Not Found</c> when the related maintenance request or equipment
    ///     cannot be found.
    /// </returns>
    /// <remarks>
    ///     If a <c>maintenanceRequestId</c> is provided, the controller validates that:
    ///     <list type="bullet">
    ///         <item>The maintenance request exists.</item>
    ///         <item>The maintenance request status is <c>completed</c>.</item>
    ///         <item>The equipment associated with the maintenance request exists.</item>
    ///     </list>
    ///     <para>Sample request body:</para>
    ///     <code language="json">
    /// {
    ///   "maintenanceRequestId": 11,
    ///   "userId": 1,
    ///   "companyName": "CoolGym Maintenance Services",
    ///   "amount": 300.00,
    ///   "currency": "USD",
    ///   "status": "pending",
    ///   "issuedAt": "2025-11-27"
    /// }
    /// </code>
    ///     <para>
    ///         Error messages are localized based on the <c>Accept-Language</c> header.
    ///     </para>
    /// </remarks>
    [HttpPost]
    public async Task<IActionResult> CreateInvoice([FromBody] CreateInvoiceResource resource)
    {
        try
        {
            if (resource.MaintenanceRequestId.HasValue)
            {
                var maintenanceQuery = new GetMaintenanceRequestById(resource.MaintenanceRequestId.Value);
                var maintenanceRequest = await maintenanceRequestQueryService.Handle(maintenanceQuery);
                if (maintenanceRequest == null)
                    return NotFound(new
                    {
                        message = localizer[
                            "Maintenance request with id {0} was not found.",
                            resource.MaintenanceRequestId.Value].Value
                    });

                if (!string.Equals(
                        maintenanceRequest.Status,
                        CompletedStatus,
                        StringComparison.OrdinalIgnoreCase))
                    return BadRequest(new
                    {
                        message = localizer["Only completed maintenance requests can generate invoices."]
                    });

                var equipmentQuery = new GetEquipmentById(maintenanceRequest.EquipmentId);
                var equipment = await equipmentQueryService.Handle(equipmentQuery);
                if (equipment == null)
                    return BadRequest(new
                    {
                        message = localizer[
                            "Equipment with id {0} was not found.",
                            maintenanceRequest.EquipmentId].Value
                    });
            }

            var command = CreateInvoiceCommandFromResourceAssembler.ToCommandFromResource(resource);
            var invoice = await invoiceCommandService.Handle(command);
            var invoiceResource = InvoiceResourceFromEntityAssembler.ToResourceFromEntity(invoice);

            return CreatedAtAction(
                nameof(GetInvoiceById),
                new { id = invoice.Id },
                invoiceResource);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = localizer[ex.Message] });
        }
        catch (FormatException)
        {
            return BadRequest(new { message = localizer["Invalid date format. Use yyyy-MM-dd."].Value });
        }
    }

    /// <summary>
    ///     Marks an existing invoice as paid.
    /// </summary>
    /// <param name="id">Identifier of the invoice to be updated.</param>
    /// <param name="resource">
    ///     Information about the payment (for example, payment date or payment method),
    ///     encapsulated in a <see cref="MarkInvoiceAsPaidResource" />.
    /// </param>
    /// <returns>
    ///     Returns <c>200 OK</c> with the updated invoice when the payment is registered correctly.
    ///     Returns <c>404 Not Found</c> when the invoice does not exist.
    ///     Returns <c>400 Bad Request</c> when domain validations are not satisfied.
    /// </returns>
    /// <remarks>
    ///     This endpoint is usually called from the billing back-office when an external
    ///     payment provider confirms that the invoice has been paid.
    /// </remarks>
    [HttpPut("{id:int}/pay")]
    public async Task<IActionResult> MarkInvoiceAsPaid(
        [FromRoute] int id,
        [FromBody] MarkInvoiceAsPaidResource resource)
    {
        try
        {
            // Get authenticated user from middleware
            var authenticatedUser = HttpContext.Items["User"] as User;

            // Authorization: User must be authenticated
            if (authenticatedUser == null)
                return Unauthorized(new { message = "Authentication required" });

            // Authorization: Must be a Client to pay invoices
            if (authenticatedUser.Role.ToRoleName() != "Client")
                return StatusCode(403, new { message = "Only clients can pay invoices" });

            // First, fetch the invoice to check ownership
            var invoiceQuery = new GetInvoiceById(id);
            var existingInvoice = await invoiceQueryService.Handle(invoiceQuery);
            if (existingInvoice == null)
                return NotFound(new { message = localizer[$"Invoice with ID {id} was not found."].Value });

            // Authorization: Clients can only pay their own invoices
            if (authenticatedUser.Id != existingInvoice.UserId)
                return StatusCode(403, new { message = "You can only pay your own invoices" });

            var command = MarkInvoiceAsPaidCommandFromResourceAssembler.ToCommandFromResource(id, resource);
            var invoice = await invoiceCommandService.Handle(command);
            var invoiceResource = InvoiceResourceFromEntityAssembler.ToResourceFromEntity(invoice);
            return Ok(invoiceResource);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { message = localizer[ex.Message].Value });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = localizer[ex.Message].Value });
        }
        catch (FormatException)
        {
            return BadRequest(new { message = localizer["Invalid date format. Use yyyy-MM-dd."].Value });
        }
    }

    /// <summary>
    ///     Deletes an invoice.
    /// </summary>
    /// <param name="id">Identifier of the invoice to be deleted.</param>
    /// <returns>
    ///     Returns <c>204 No Content</c> when the invoice is successfully deleted.
    ///     Returns <c>404 Not Found</c> when no invoice exists with the provided identifier.
    /// </returns>
    /// <remarks>
    ///     Deleting an invoice should be used carefully and usually only in exceptional
    ///     scenarios (for example, test data cleanup or administrative corrections).
    /// </remarks>
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteInvoice([FromRoute] int id)
    {
        var command = new DeleteInvoiceCommand(id);
        var success = await invoiceCommandService.Handle(command);
        if (!success) return NotFound(new { message = localizer[$"Invoice with ID {id} was not found."].Value });

        return NoContent();
    }
}