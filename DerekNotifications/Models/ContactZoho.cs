using System.Text.Json.Serialization;

namespace DerekNotifications.Models;

public class ContactZoho
{
    [JsonPropertyName("contact_id")] 
    public string? Id { get; set; }
    
    [JsonPropertyName("contact_name")] 
    public string? Name { get; set; }
}