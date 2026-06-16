using System.Text.Json;
using DerekNotifications.Models.Requests;
using DerekNotifications.Models.Responses;
using OpenAI.Responses;

namespace DerekNotifications.Services;

public interface IGenerateFollowUpService
{
    Task<GenerateEmailResponse> GenerateFollowUp(GenerateFollowUpRequest request);
}

public class GenerateFollowUpService(
    OpenAIResponseClient openAiResponseClient
) : IGenerateFollowUpService
{
public async Task<GenerateEmailResponse> GenerateFollowUp(GenerateFollowUpRequest request)
        {
            var notesText = string.Join("\n", request.Context.Notes.Select(note =>
                $"- {note.DateCreated}: {note.Note}"
            ));

            var response =
                await openAiResponseClient.CreateResponseAsync($$"""
                                                                You are a helpful assistant that generates follow-up emails for customers.
                                                                You are not super smart, but you are good at writing emails.
                                                                You always write emails that are friendly but not over the top corporate.
                                                                You never Talk about someones knowledge or experience.

                                                                Use the CRM notes below to write a personalized follow-up email.
                                                                Do not ask the user for more details unless the CRM notes are empty or unusable.

                                                                Return only valid JSON in this exact shape:
                                                                {
                                                                  "subject": "Email subject here",
                                                                  "body": "Email body here"
                                                                }
                                                                
                                                                Make sure to look at the Task to generate a follow-up that is on par with the task.
                                                                Here is the task:
                                                                {{request.Context.Task}}

                                                                Do not wrap the JSON in markdown.
                                                                Do not include any explanation outside the JSON.

                                                                CRM notes:
                                                                {{notesText}}

                                                                Sign off Details:
                                                                Jesse Raibourn
                                                                Roughneck Software
                                                                972-777-5958

                                                                Customer:
                                                                {{request.Context.ContactName}}
                                                                """);

            var outputText = response.Value.GetOutputText();

            var generatedEmail = JsonSerializer.Deserialize<GenerateEmailResponse>(
                outputText,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        return generatedEmail ?? new GenerateEmailResponse();
    }
}