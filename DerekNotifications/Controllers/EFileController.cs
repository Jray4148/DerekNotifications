using DerekNotifications.Attributes;
using DerekNotifications.Interfaces;
using DerekNotifications.Models.Requests;
using DerekNotifications.Services;
using Microsoft.AspNetCore.Mvc;

namespace DerekNotifications.Controllers;

[ApiController]
[Route("[controller]")]
public class EFileController(
    IEmailService emailService,
    ITicketsService ticketsService,
    IDynamoService dynamoService,
    ILogger<EFileController> logger) : ControllerBase
{
    [ApiKey]
    [HttpGet("v1/understand")]
    public IActionResult Understand([FromQuery] EFileUnderstandRequest request)
    {
        request.Type = Constants.RequestType.EFileUnderstand;
        Task.Run(() => emailService.ProcessRequestAsync(request));
        Task.Run(() => ticketsService.CreateAsync(request));
        return Ok();
    }

    [ApiKey]
    [HttpGet("v1/billme")]
    public async Task<string> BillMe([FromQuery] EFileBillMeRequest request)
    {
        request.Type = Constants.RequestType.EFileBillMe;
        await emailService.ProcessRequestAsync(request);
    
        try
        {
            await ticketsService.CreateAsync(request);
            await dynamoService.CreateAsync(request);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed BillMe, Request: {@Request}", request);
            return "Error: " + ex.Message;
        }        
        
        if (string.Equals(request.Action, "bill_me_confirmed", StringComparison.OrdinalIgnoreCase))
        {
            var boid = request.BOID;

            if (!string.IsNullOrWhiteSpace(boid))
            {
                _ = RunDetached(
                    () => ticketsService.UpdateBillMeTicketsConfirmationForBatchOidAsync(boid),
                    ex => logger.LogError(ex, "Background update failed for BOID {BOID}", boid),
                    timeout: TimeSpan.FromSeconds(30),
                    onTimeout: () => logger.LogWarning("Background update timed out for BOID {BOID}", boid));            }
            else
            {
                logger.LogWarning("bill_me_confirmed action received but BOID was missing.");
            }
        }

        return "";
    }

    private static async Task RunDetached(
        Func<Task> work,
        Action<Exception> onError,
        TimeSpan timeout,
        Action onTimeout)
    {
        try
        {
            var workTask = work();

            var completed = await Task.WhenAny(workTask, Task.Delay(timeout)).ConfigureAwait(false);
            if (completed != workTask)
            {
                onTimeout();
                return;
            }

            await workTask.ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            onError(ex);
        }
    }

    // This was just being used for testing
    // [HttpPost("v1/billme-confirmation-check")]
    // public async Task<IEnumerable<BillMeTicket>> HasConfirmationCheck()
    // {
    //     return await ticketsService.HasConfirmationCheck();        
    // }
}