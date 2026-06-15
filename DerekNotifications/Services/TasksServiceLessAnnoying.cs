using System.Text;
using System.Text.Json;
using DerekNotifications.Models.Requests;
using DerekNotifications.Models.Responses;

namespace DerekNotifications.Services;

public interface ITasksServiceLessAnnoying
{
    Task<LessAnnoyingTasksResponse> GetAsync(LessAnnoyingTaskRequest request);
}

public class TasksServiceLessAnnoying(
    HttpClient httpClient
) : ITasksServiceLessAnnoying
{
        public async Task<LessAnnoyingTasksResponse> GetAsync(LessAnnoyingTaskRequest request)
        {
            var lacrmRequest = new
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
}