using System.Text.Json.Serialization;
using DerekNotifications.Models.Requests;

namespace DerekNotifications.Models.Issues;

public abstract class Issue(Request request)
{
    protected Request Request { get; set;  } = request;
    
    public string? Url { get; set; } = Constants.Yt.Url;
    
    [JsonPropertyName("project")]
    public ProjectYouTrack? Project { get; set; }

    [JsonPropertyName("summary")]
    public string? Summary { get; set; }
    
    [JsonPropertyName("description")]
    public string? Description { get; set; }
    
    [JsonPropertyName("customFields")]
    public List<object>? CustomFields { get; set; }
}