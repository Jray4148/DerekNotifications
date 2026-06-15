using System.Text.Json.Serialization;

namespace DerekNotifications.Models.Issues;

public class CustomField
{
    [JsonPropertyName("$type")]
    public string? Type { get; set; }
    
    [JsonPropertyName("name")]
    public string? Name { get; set; }
    
    [JsonPropertyName("value")]
    public object? Value { get; set; }
}