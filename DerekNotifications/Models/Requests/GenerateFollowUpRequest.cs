namespace DerekNotifications.Models.Requests;

public class GenerateFollowUpRequest
{
    public GenerateFollowUpDetails Context { get; set; } = new();
}

public class GenerateFollowUpDetails
{
    public string? ContactName { get; set; }
    public List<Notes> Notes { get; set; } = new();
    public string? Task { get; set; }
}

public class Notes
{
    public string? Note { get; set; }
    public string? DateCreated { get; set; }
}


