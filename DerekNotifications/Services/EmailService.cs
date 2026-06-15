using Amazon.SimpleEmail;
using Amazon.SimpleEmail.Model;
using DerekNotifications.Interfaces;
using DerekNotifications.Models.Emails;
using DerekNotifications.Models.Requests;

namespace DerekNotifications.Services;

public interface IEmailService
{
    Task ProcessRequestAsync(Request request);
}

public class EmailService(
    IEmailFactory emailFactory,
    ILogger<EmailService> logger) : IEmailService
{
    public async Task ProcessRequestAsync(Request request)
    {
        try
        {
            var emails = emailFactory.Create(request);
            
            foreach (var email in emails)
            {
                await SendEmailAsync(email);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to send Derek registration email, Request: {@Request}", request);
        }
    }

    private async Task SendEmailAsync(Email email)
    {
        var subject = email.Subject;
        var client = new AmazonSimpleEmailServiceClient();

        var sendRequest = new SendEmailRequest
        {
            Source = email.From,
            Destination = new Destination
            {
                ToAddresses = [email.To]
            },
            Message = new Message
            {
                Subject = new Content(subject),
                Body = new Body
                {
                    Text = new Content(email.Body),
                }
            }
        };

        try
        {
            var response = await client.SendEmailAsync(sendRequest);
            logger.LogInformation("Email to {sendTo} sent. Message ID: {response.MessageId}", email.To, response.MessageId);
        }
        catch (Exception ex)
        {
            logger.LogError("Email to {sendTo} failed. Message ID: {ex.Message}", email.To, ex.Message);
        }        
    }
}
