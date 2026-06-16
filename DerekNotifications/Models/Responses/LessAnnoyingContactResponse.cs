namespace DerekNotifications.Models.Responses;

public class LessAnnoyingContactResponse
{
    public string ContactId { get; set; } = string.Empty;
    public List<LessAnnoyingContactEmail> Email { get; set; } = [];
}

public class LessAnnoyingContactEmail
{
    public string Text { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string TypeId { get; set; } = string.Empty;
}