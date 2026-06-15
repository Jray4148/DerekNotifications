namespace DerekNotifications.Models.Requests;

public class DerekRegisterRequest : Request
{
    public override Constants.RequestType Type { get; set; } = Constants.RequestType.DerekRegister;
    public string? CoName { get; set; }
    public string? SN { get; set; }
    public string? RN { get; set; }
    public string? Email { get; set; }
    public string? RegistrationKey { get; set; }
}
