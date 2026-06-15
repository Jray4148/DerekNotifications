using System.Text.Json.Serialization;

namespace DerekNotifications.Models.Issues;

public class ProjectYouTrack
{
    [JsonPropertyName("id")]
    public required string Id { get; set; }
}