using DerekNotifications.Interfaces;
using DerekNotifications.Models;
using DerekNotifications.Models.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DerekNotifications.Controllers;

[ApiController]
[Authorize]
[Route("[controller]")]
public class InvoicesController(
    IInvoicesServices invoiceService,
    ITicketsService ticketsService) : ControllerBase
{
    /// <summary>
    /// Builds invoices for a specific project based on the provided project ID.
    /// Retrieves tickets associated with the specified project and generates a list of invoices.
    /// No invoice creation is performed.
    /// </summary>
    /// <param name="tickets"></param>
    /// <returns>
    /// A task representing the asynchronous operation, containing a list of invoices generated for the project.
    /// </returns>
    [HttpPost("preview")]
    public async Task<BuildInvoicesResult> Preview([FromBody] List<BillMeTicket> tickets)
    {
        return await invoiceService.BuildInvoicesForSerialNumber(tickets);
    }
    
    // TODO: DO WE USE THIS?   
    // [HttpGet("{projectId}")]
    // public async Task<List<Invoice>> BuildForProject(string projectId)
    // {
    //     var tickets = await ticketsService.GetBillMeTicketsAsync(new EFileBillMeFilterRequest { ProjectId = projectId });
    //     return await invoiceService.BuildInvoicesForSerialNumber(tickets);
    // }    
    
    /// <summary>
    /// Creates invoices for a specific project based on the provided project ID and request data.
    /// The request data contains a list of ticket IDs used to filter the tickets for invoice creation.
    /// </summary>
    /// <param name="projectId">
    /// The unique identifier of the project for which invoices are to be created.
    /// </param>
    /// <param name="request">
    /// The request object containing the ticket IDs used to create the invoices.
    /// </param>
    /// <returns>
    /// An HTTP result indicating the success or failure of the operation.
    /// </returns>
    [HttpPost("{projectId}")]
    public async Task<IActionResult> CreateForProject(string projectId, [FromBody] InvoicesCreateRequest request)
    {
        var tickets = await ticketsService.GetTicketsForIdsAsync(request.TicketIds);
        var result = tickets.Select(BillMeTicket.Map)
            .OfType<BillMeTicket>()
            .ToList();
        
        await invoiceService.CreateInvoicesForSerialNumber(result, request.TicketIds);
        return Ok();
    }
}