using DerekNotifications.Interfaces;
using DerekNotifications.Models.Requests;

namespace DerekNotifications.Services;

public interface IRegisterService
{
    Task<string> HandleRegistrationAsync(Request request, Constants.RequestType requestType, string product);
    bool TryCreateRegistrationKey(DerekRegisterRequest request);
}

public class RegisterService(
    IEmailService emailService,
    ITicketsService ticketsService,
    ILogger<EmailService> logger) : IRegisterService
{
    public async Task<string> HandleRegistrationAsync(
        Request request, 
        Constants.RequestType requestType, 
        string product)
    {
        request.Type = requestType;
        await emailService.ProcessRequestAsync(request);
        await ticketsService.CreateAsync(request);
        return ReplyMessage(product);
    }    
    
    /// <summary>
    /// Attempts to create a registration key for a given Derek registration request.
    /// This key is just a re-encrypting of the registration number provided in the request plus
    /// adding "RSI" to the string.
    /// </summary>
    /// <param name="request">The DerekRegisterRequest object that contains the registration number
    /// and other relevant registration details.</param>
    /// <returns>A boolean value indicating whether the registration key was successfully created.
    /// Returns true if the key was created successfully or false if an error occurred.</returns>
    public bool TryCreateRegistrationKey(DerekRegisterRequest request)
    {
        try
        {
            request.RegistrationKey = CryptUtil.CreateKey(request.RN!);
            return true;
        }
        catch (Exception ex)
        {
            logger.LogError("Error creating registration key { ex }", ex);
            return false;
        }
    }

    private static string ReplyMessage(string productName)
    {
        return $"""
                Dear Roughneck Customer,

                Thank you for registering for {productName}.  You will be receiving a confirmation email.  
                If you do not receive this email within a few hours, please try to register your software again.

                If you have any problems please e-mail or give us a call.

                Thank you,
                RSI
                972-552-5204
                register@roughnecksoftware.com
                <HTML>
                """;
    }
}