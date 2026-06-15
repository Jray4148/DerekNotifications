namespace DerekNotifications.Models.Requests;

public class AskDerekRequest : Request
{
    public override Constants.RequestType Type { get; set; } = Constants.RequestType.AskDerek;
    public string? Question { get; set; }
    public string? Answer { get; set; }
    public string? AskedBy { get; set; }
}