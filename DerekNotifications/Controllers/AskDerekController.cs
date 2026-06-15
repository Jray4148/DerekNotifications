using DerekNotifications.Interfaces;
using DerekNotifications.Models.Requests;
using Microsoft.AspNetCore.Mvc;

namespace DerekNotifications.Controllers;

[ApiController]
[Route("[controller]")]
public class AskDerekController(
    ITicketsService ticketsService) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Ask([FromBody] AskDerekRequest request)
    {
        await ticketsService.CreateAsync(request);
        return Ok();
    }
}