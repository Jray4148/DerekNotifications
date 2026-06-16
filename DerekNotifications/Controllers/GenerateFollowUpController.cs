using DerekNotifications.Models.Requests;
using DerekNotifications.Services;
using Microsoft.AspNetCore.Mvc;

namespace DerekNotifications.Controllers;

[ApiController]
[Route("generate-follow-up")]
public class GenerateFollowUpController(
    IGenerateFollowUpService generateFollowUpService,
    ITasksServiceLessAnnoying tasksServiceLessAnnoying) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Generate([FromBody] GenerateFollowUpRequest request)
    {
        var response = await generateFollowUpService.GenerateFollowUp(request);
        return Ok(response);
    }
    
    [HttpPost("send-email")]
    public async Task<IActionResult> SendEmail([FromBody] EmailRequest emailRequest)
    {
        await tasksServiceLessAnnoying.SendEmailAsync(emailRequest);
        return Ok();
    }
}