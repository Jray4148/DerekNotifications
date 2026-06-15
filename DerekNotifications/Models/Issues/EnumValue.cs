using System.Text.Json.Serialization;

namespace DerekNotifications.Models.Issues;

public class EnumValue
{
    [JsonPropertyName("$type")]
    public string? Type { get; set; }
    
    [JsonPropertyName("name")]
    public string? Name { get; set; }
}