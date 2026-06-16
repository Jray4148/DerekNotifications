using DerekNotifications.Models.Requests;
using Microsoft.AspNetCore.Mvc;

namespace DerekNotifications.Controllers;

[ApiController]
[Route("generate-follow-up")]
public class GenerateFollowUpController() : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Generate([FromBody] GenerateFollowUpRequest request)
    {
        return Ok();
    }
}