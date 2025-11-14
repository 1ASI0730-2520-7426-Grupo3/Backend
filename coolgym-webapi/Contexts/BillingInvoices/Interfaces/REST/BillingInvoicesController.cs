using coolgym_webapi.Contexts.BillingInvoices.Domain.Commands;
using coolgym_webapi.Contexts.BillingInvoices.Domain.Queries;
using coolgym_webapi.Contexts.BillingInvoices.Domain.Services;
using coolgym_webapi.Contexts.BillingInvoices.Interfaces.REST.Resources;
using coolgym_webapi.Contexts.BillingInvoices.Interfaces.REST.Transform;
using Microsoft.AspNetCore.Mvc;

namespace coolgym_webapi.Contexts.BillingInvoices.Interfaces.REST;

/// <summary>
/// REST API Controller for billing invoices
/// Provides endpoints for managing and viewing user invoices
/// </summary>
[ApiController]
[Route("api/v1/billing/invoices")]
public class BillingInvoicesController(
    IInvoiceQueryService invoiceQueryService,
    IInvoiceCommandService invoiceCommandService) : ControllerBase
{
    /// <summary>
    /// Get all invoices for a specific user
    /// </summary>
    /// <param name="userId">The user ID to filter invoices</param>
    /// <returns>List of invoices for the user</returns>
    [HttpGet]
    public async Task<IActionResult> GetInvoicesByUserId([FromQuery] int userId)
    {
        if (userId <= 0)
            return BadRequest(new { message = "User ID must be positive" });

        var query = new GetInvoicesByUserId(userId);
        var invoices = await invoiceQueryService.Handle(query);
        var resources = invoices.Select(InvoiceResourceFromEntityAssembler.ToResourceFromEntity);

        return Ok(resources);
    }

    /// <summary>
    /// Get a specific invoice by ID
    /// </summary>
    /// <param name="id">Invoice ID</param>
    /// <returns>Invoice details or 404 if not found</returns>
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetInvoiceById([FromRoute] int id)
    {
        var query = new GetInvoiceById(id);
        var invoice = await invoiceQueryService.Handle(query);

        if (invoice == null)
            return NotFound(new { message = $"Invoice with ID {id} not found" });

        var resource = InvoiceResourceFromEntityAssembler.ToResourceFromEntity(invoice);
        return Ok(resource);
    }

    /// <summary>
    /// Get all invoices (for admin purposes)
    /// </summary>
    /// <returns>List of all invoices</returns>
    [HttpGet("all")]
    public async Task<IActionResult> GetAllInvoices()
    {
        var query = new GetAllInvoices();
        var invoices = await invoiceQueryService.Handle(query);
        var resources = invoices.Select(InvoiceResourceFromEntityAssembler.ToResourceFromEntity);

        return Ok(resources);
    }

    /// <summary>
    /// Create a new billing invoice
    /// </summary>
    /// <param name="resource">Invoice creation data</param>
    /// <returns>Created invoice or 400 if validation fails</returns>
    [HttpPost]
    public async Task<IActionResult> CreateInvoice([FromBody] CreateInvoiceResource resource)
    {
        try
        {
            var command = CreateInvoiceCommandFromResourceAssembler.ToCommandFromResource(resource);
            var invoice = await invoiceCommandService.Handle(command);
            var invoiceResource = InvoiceResourceFromEntityAssembler.ToResourceFromEntity(invoice);

            return CreatedAtAction(
                nameof(GetInvoiceById),
                new { id = invoice.Id },
                invoiceResource
            );
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (FormatException)
        {
            return BadRequest(new { message = "Invalid date format. Use yyyy-MM-dd" });
        }
    }

    /// <summary>
    /// Mark an invoice as paid
    /// </summary>
    /// <param name="id">Invoice ID</param>
    /// <param name="resource">Payment date information</param>
    /// <returns>Updated invoice or error</returns>
    [HttpPut("{id:int}/pay")]
    public async Task<IActionResult> MarkInvoiceAsPaid(
        [FromRoute] int id,
        [FromBody] MarkInvoiceAsPaidResource resource)
    {
        try
        {
            var command = MarkInvoiceAsPaidCommandFromResourceAssembler.ToCommandFromResource(id, resource);
            var invoice = await invoiceCommandService.Handle(command);
            var invoiceResource = InvoiceResourceFromEntityAssembler.ToResourceFromEntity(invoice);

            return Ok(invoiceResource);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (FormatException)
        {
            return BadRequest(new { message = "Invalid date format. Use yyyy-MM-dd" });
        }
    }

    /// <summary>
    /// Delete an invoice (soft delete)
    /// </summary>
    /// <param name="id">Invoice ID</param>
    /// <returns>NoContent if successful, NotFound if invoice doesn't exist</returns>
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteInvoice([FromRoute] int id)
    {
        var command = new DeleteInvoiceCommand(id);
        var success = await invoiceCommandService.Handle(command);

        if (!success)
            return NotFound(new { message = $"Invoice with ID {id} not found" });

        return NoContent();
    }
}
