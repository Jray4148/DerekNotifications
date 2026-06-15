using System.Text;
using System.Text.Json;
using DerekNotifications.Models.Requests;
using DerekNotifications.Models.Responses;

namespace DerekNotifications.Services;

public interface ITasksServiceLessAnnoying
{
    Task<LessAnnoyingTasksResponse> GetAsync(LessAnnoyingTaskRequest request);
    Task<string> GetDetailsAsync(string contactId);
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

        using var content = new StringContent(json, Encoding.UTF8, "application/json");

        using var response = await httpClient.PostAsync("", content);

        var responseBody = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception("failed to get tasks: " + responseBody + "");
        }

        var result = JsonSerializer.Deserialize<LessAnnoyingTasksResponse>(
            responseBody,
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

        return result ?? throw new Exception("failed to deserialize tasks response");
    }
    
    public async Task<string> GetDetailsAsync(string contactId)
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
            
            using var content = new StringContent(json, Encoding.UTF8, "application/json");
            
            using var response = await httpClient.PostAsync("", content);
            
            var responseBody = await response.Content.ReadAsStringAsync();
            
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("failed to get tasks: " + responseBody + "");
            }

            return responseBody;
    }
}