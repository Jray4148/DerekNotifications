using DerekNotifications.Interfaces;
using DerekNotifications.Models.Issues;
using DerekNotifications.Models.Requests;
using Microsoft.Extensions.Options;

namespace DerekNotifications.Services;

public partial class TicketsServiceYouTrack(
    IIssueFactoryYouTrack issueFactoryYouTrack,
    HttpClient httpClient,
    IOptions<AppSettingsService> appSettingsService,
    ILogger<TicketsServiceYouTrack> logger) : ITicketsService
{
    /// <summary>
    /// Creates a new issue in the YouTrack system based on the provided request.
    /// </summary>
    /// <param name="request">The request object containing information about the issue to be created.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task CreateAsync(Request request)
    {
        try
        {
            var issue = issueFactoryYouTrack.Create(request);
            using var response = await httpClient.PostAsJsonAsync("", issue);
            if (response.IsSuccessStatusCode)
            {
                logger.LogInformation("Issue created: {status}", response.StatusCode);
                return;
            };
            var errorContent = await response.Content.ReadAsStringAsync();
            logger.LogError("YouTrack API failed. Status: {StatusCode}, Reason: {ErrorContent}", response.StatusCode, errorContent);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to send issue, Request: {@Request}", request);
            throw;
        }
    }

    /// <summary>
    /// Helper method to update the state of a specified issue in the YouTrack system.
    /// </summary>
    /// <param name="issueId">The unique identifier of the issue to be updated.</param>
    /// <param name="state">The new state to be applied to the issue.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task UpdateStateAsync(string issueId, string state)
    {
        var payload = CreateIssueUpdatePayload("State", new EnumValue { Name = state }, "StateIssueCustomField");
        await UpdateIssueAsync(issueId, payload);
    }
    
    private async Task<Uri> BuildSearchRequestUri(string searchExpression, string responseFields)
    {
        var queryString = await new FormUrlEncodedContent(new Dictionary<string, string>
        {
            { "fields", responseFields },
            { "query", searchExpression },
            { "$top", "200" },
            { "$skip", "0"}
        }).ReadAsStringAsync();
        
        return new UriBuilder(httpClient.BaseAddress!)
        {
            Query = queryString
        }.Uri;
    }

    private async Task UpdateIssueAsync(string issueId, object payload)
    {
        try
        {
            using var response = await httpClient.PostAsJsonAsync(issueId, payload);
            response.EnsureSuccessStatusCode();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error updating {issueId}", issueId);
        }        
    }

    private static object CreateIssueUpdatePayload(string fieldName, object value, string fieldType = "SimpleIssueCustomField")
    {
        return new
        {
            customFields = new[]
            {
                new CustomField
                {
                    Type = fieldType,
                    Name = fieldName,
                    Value = value
                }
            }
        };
    }    
}