namespace DerekNotifications.Models.Requests;

public class EmailRequest
{
    public string Subject { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public string EmailId { get; set; } = string.Empty;
    public string ContactId { get; set; } = string.Empty;
}