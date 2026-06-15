using Microsoft.AspNetCore.Mvc;

namespace DerekNotifications.Controllers;

[ApiController]
[Route("[controller]")]
public class TasksController() : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        return Ok();
    }
}