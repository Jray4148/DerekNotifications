using DerekNotifications.Models.Requests;
using DerekNotifications.Services;
using Microsoft.AspNetCore.Mvc;

namespace DerekNotifications.Controllers;

[ApiController]
[Route("[controller]")]
public class RegisterController(
    IRegisterService registerService,
    ILogger<RegisterController> logger) : ControllerBase
{
    /// <summary>
    /// Handles the Derek registration process. This includes validation, creating a registration key,
    /// and processing the request through email and issue services.
    /// </summary>
    /// <param name="request">The DerekRegisterRequest object containing registration details such as company name,
    /// serial number, registration number, email, and other relevant fields.</param>
    /// <returns>A string message indicating the registration result or error information.</returns>
    [HttpGet("v1/derek")]
    public async Task<string> DerekRegister([FromQuery] DerekRegisterRequest request)
    {
        if (!ValidateRegistrationRequest(request))
        {
            logger.LogError("Registration Number not provided { company }", request.CoName ?? "Unknown");
            return "Registration Number is required";
        }

        if (!registerService.TryCreateRegistrationKey(request))
        {
            return "Error creating registration key";
        }

        return await registerService.HandleRegistrationAsync(
            request, 
            Constants.RequestType.DerekRegister, 
            Constants.Products.Derek);
    }

    /// <summary>
    /// Handles the EBS registration process. This includes setting the request type,
    /// and processing the registration details through email and issue services.
    /// </summary>
    /// <param name="request">The EbsRegisterRequest object containing registration details such as company name,
    /// serial number, registration number, email, username, and password.</param>
    /// <returns>A string message indicating the registration result or confirmation information.</returns>
    [HttpGet("v1/ebs")]
    public async Task<string> EbsRegister([FromQuery] EbsRegisterRequest request)
    {
        return await registerService.HandleRegistrationAsync(
            request, 
            Constants.RequestType.EbsRegister, 
            Constants.Products.Ebs);
    }
    
    private static bool ValidateRegistrationRequest(DerekRegisterRequest request)
    {
        return !string.IsNullOrEmpty(request?.RN);
    }
}