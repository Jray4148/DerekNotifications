using System.Text.Json.Serialization;

namespace DerekNotifications.Models.Responses;

public class ZohoContactsResponse
{
    [JsonPropertyName("code")] 
    public int Code { get; set; }
    
    [JsonPropertyName("message")]
    public string? Message { get; set; }
    
    [JsonPropertyName("contacts")]
    public List<ContactZoho>? Contacts { get; set; }    
}