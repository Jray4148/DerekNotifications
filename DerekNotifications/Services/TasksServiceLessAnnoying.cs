using System.Text;
using System.Text.Json;
using Amazon.SimpleEmail;
using Amazon.SimpleEmail.Model;
using DerekNotifications.Models.Requests;
using DerekNotifications.Models.Responses;

namespace DerekNotifications.Services;

public interface ITasksServiceLessAnnoying
{
    Task<LessAnnoyingTasksResponse> GetAsync(LessAnnoyingTaskRequest request);
    Task<LessAnnoyingNotesResponse> GetDetailsAsync(string contactId);
    Task SendEmailAsync(EmailRequest email);
}

public class TasksServiceLessAnnoying(
    HttpClient httpClient
) : ITasksServiceLessAnnoying
{
    /// <summary>
    /// Retrieves tasks based on the specified request parameters.
    /// </summary>
    /// <param name="request">The request containing parameters such as the start date, end date, and user filter for fetching tasks.</param>
    /// <returns>A <see cref="LessAnnoyingTasksResponse"/> object containing the tasks data.</returns>
    /// <exception cref="Exception">Thrown when the HTTP request fails or the response cannot be deserialized.</exception>
    public async Task<LessAnnoyingTasksResponse> GetAsync(LessAnnoyingTaskRequest request)
    {
        var lacrmRequest = new LessAnnoyingApiRequest
        {
            Function = "GetTasks",
            Parameters = new
            {
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                UserFilter = new List<string> { "612369" }
            }
        };
            
        var json = JsonSerializer.Serialize(lacrmRequest);
        var responseBody = await SendPostAsync(json);
        
       return await DeserializeAsync<LessAnnoyingTasksResponse>(responseBody);       
    }

    /// <summary>
    /// Retrieves notes attached to a specific contact using the provided contact ID.
    /// </summary>
    /// <param name="contactId">The unique identifier of the contact for which notes are to be fetched.</param>
    /// <returns>A <see cref="LessAnnoyingNotesResponse"/> instance containing the notes data associated with the specified contact.</returns>
    /// <exception cref="Exception">Thrown if the HTTP request fails or the response deserialization encounters an error.</exception>
    public async Task<LessAnnoyingNotesResponse> GetDetailsAsync(string contactId)
    {
            var lacrmRequest = new LessAnnoyingApiRequest
            {
                Function = "GetNotesAttachedToContact",
                Parameters = new
                {
                    ContactId = contactId
                }
            };
            
            var json = JsonSerializer.Serialize(lacrmRequest);
            var responseBody = await SendPostAsync(json);
            return await DeserializeAsync<LessAnnoyingNotesResponse>(responseBody);
    }


    /// <summary>
    /// Retrieves the email address associated with the specified contact ID.
    /// </summary>
    /// <param name="contactId">The unique identifier of the contact for which the email address is being retrieved.</param>
    /// <returns>The email address of the contact as a string.</returns>
    /// <exception cref="Exception">Thrown when no email address is found for the specified contact ID or if the API request fails.</exception>
    private async Task<string> GetEmailAddressAsync(string contactId)
    {
        var lacrmRequest = new LessAnnoyingApiRequest
        {
            Function = "GetContact",
            Parameters = new
            {
                ContactId = contactId,
            }
        };

        var json = JsonSerializer.Serialize(lacrmRequest);
        var responseBody = await SendPostAsync(json);
        var contact = await DeserializeAsync<LessAnnoyingContactResponse>(responseBody);

        return contact.Email.FirstOrDefault()?.Text
               ?? throw new Exception($"No email address found for contact {contactId}");
    }

    /// <summary>
    /// Logs an email in the Less Annoying CRM system based on the provided email request details.
    /// </summary>
    /// <param name="emailRequest">The request containing details such as the contact ID, subject, and body of the email to be logged.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    /// <exception cref="Exception">Thrown when the request to the Less Annoying CRM API fails or encounters an issue.</exception>
    private async Task LogEmailInLessAnnoyingAsync(EmailRequest emailRequest)
    {
        var emailAddress = await GetEmailAddressAsync(emailRequest.ContactId);

        var lacrmRequest = new LessAnnoyingApiRequest
        {
            Function = "CreateEmail",
            Parameters = new
            {
                ContactIds = new List<string> { emailRequest.ContactId },
                UserIsSender = true,
                From = new
                {
                    Address = "jesse@roughnecksoftware.com",
                    Name = "Roughneck Software"
                },
                To = new[]
                {
                    new
                    {
                        Address = emailAddress
                    }
                },
                Cc = new[]
                {
                    new
                    {
                        Address = "jesse@roughnecksoftware.com"
                    }
                },
                Subject = emailRequest.Subject,
                Body = emailRequest.Body,
                Date = DateTime.Now.ToString("yyyy-MM-dd"),
            }
        };

        var json = JsonSerializer.Serialize(lacrmRequest);
        await SendPostAsync(json);
    }

    /// <summary>
    /// Sends a POST request with the specified JSON payload and retrieves the response as a string.
    /// </summary>
    /// <param name="json">The JSON string to be included in the body of the POST request.</param>
    /// <returns>A string containing the response body from the POST request.</returns>
    /// <exception cref="Exception">Thrown when the HTTP request fails or the response has an unsuccessful status code.</exception>
    private async Task<string> SendPostAsync(string json)
    {
        using var content = new StringContent(json, Encoding.UTF8, "application/json");
        using var response = await httpClient.PostAsync("", content);
        
        if (!response.IsSuccessStatusCode)
            throw new Exception("LessAnnoying Api Request Failed: " + response + "");
        
        return await response.Content.ReadAsStringAsync();
    }

    /// <summary>
    /// Deserializes the provided JSON response body into an object of the specified type.
    /// </summary>
    /// <param name="responseBody">The JSON string to be deserialized.</param>
    /// <typeparam name="T">The type to which the JSON will be deserialized.</typeparam>
    /// <returns>An object of type <typeparamref name="T"/> deserialized from the JSON string.</returns>
    /// <exception cref="Exception">Thrown when deserialization fails or the response is invalid.</exception>
    private Task<T> DeserializeAsync<T>(string responseBody)
    {
        try
        {
            var result = JsonSerializer.Deserialize<T>(responseBody, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            return Task.FromResult(result ?? throw new Exception("failed to deserialize response"));
        }
        catch (Exception exception)
        {
            return Task.FromException<T>(exception);
        }
    }

    /// <summary>
    /// Sends an email using the provided email request details, including the contact ID, subject, and body.
    /// </summary>
    /// <param name="email">The request containing the contact ID, subject, and body for the email.</param>
    /// <returns>A task that represents the asynchronous operation of sending the email.</returns>
    /// <exception cref="Exception">Thrown when the email fails to send or an error occurs during the operation.</exception>
    public async Task SendEmailAsync(EmailRequest email)
    {
        var emailAddress = await GetEmailAddressAsync(email.ContactId);
        var client = new AmazonSimpleEmailServiceClient();

        var sendRequest = new SendEmailRequest
        {
            Source = "jesse@roughnecksoftware.com",
            Destination = new Destination
            {
                ToAddresses = [emailAddress],
                CcAddresses = ["jesse@roughnecksoftware.com"]
            },
            Message = new Message
            {
                Subject = new Content(email.Subject),
                Body = new Body
                {
                    Text = new Content(email.Body),
                }
            }
        };

        try
        {
            await client.SendEmailAsync(sendRequest);
            await LogEmailInLessAnnoyingAsync(email);
        }
        catch (Exception ex)
        {
            throw new Exception("Email failed to send email" + ex.Message);
        }        
    }
}