using DerekNotifications.Models.Requests;
using DerekNotifications.Services;
using Microsoft.AspNetCore.Mvc;

namespace DerekNotifications.Controllers;

[ApiController]
[Route("[controller]")]
public class TasksController(ITasksServiceLessAnnoying tasksServiceLessAnnoying) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] LessAnnoyingTaskRequest request)
    {
        var tasks = await tasksServiceLessAnnoying.GetAsync(request);
        return Ok(tasks);
    }
}