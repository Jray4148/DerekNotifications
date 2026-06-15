using Microsoft.AspNetCore.Mvc;

namespace DerekNotifications.Controllers;

[ApiController]
[Route("[controller]")]
public class InfoController() : ControllerBase
{
    [HttpGet("health")]
    public IActionResult Health()
    {
        return Ok();
    }
}