namespace DerekNotifications.Models.Requests;

public class LessAnnoyingTaskRequest
{
    public required string StartDate { get; set; }
    public required string EndDate { get; set; }
}