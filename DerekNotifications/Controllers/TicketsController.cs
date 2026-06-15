using DerekNotifications.Interfaces;
using DerekNotifications.Models;
using DerekNotifications.Models.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DerekNotifications.Controllers;

[ApiController]
[Authorize]
[Route("[controller]")]
public class TicketsController(
    ITicketsService ticketsService,
    IDynamoService dynamoService) : ControllerBase
{
    [HttpGet("paged")]
    public async Task<List<BillMeTicket>> GetPaged([FromQuery] PagedRequest pagedRequest)
    {
        return await ticketsService.GetBillMeTicketsAsync(pagedRequest);
    }
    
    [HttpGet("count")]
    public async Task<long> Count()
    {
        return await ticketsService.GetTicketCount();
    }

    [AllowAnonymous]
    [HttpGet("dynamo")]
    public async Task<List<BillMeTicketDynamo>> GetDynamo([FromQuery] TicketsRequestDynamo request)
    {
        var result = await dynamoService.GetByBatchOidAsync(request.BatchOid!);
        return result != null ? [result] : [];
        // return await dynamoService.GetByFormTypeForDayUtcAsync(request);
        // return await dynamoService.GetBillMeTicketsAsync(request);
    }
 }