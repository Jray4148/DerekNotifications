namespace DerekNotifications.Models.Requests;

public class LessAnnoyingApiRequest
{
    public required string Function { get; set; }
    public required object Parameters { get; set; }
}