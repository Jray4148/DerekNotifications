namespace DerekNotifications.Models.Requests;

public class Request
{
    public virtual Constants.RequestType Type { get; set; } = Constants.RequestType.Unknown;
}