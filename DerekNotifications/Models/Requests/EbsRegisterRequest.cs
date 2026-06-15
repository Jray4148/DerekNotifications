namespace DerekNotifications.Models.Requests;

public class EbsRegisterRequest : Request
{
    public override Constants.RequestType Type { get; set; } = Constants.RequestType.EbsRegister;
    public string? CoName { get; set; }
    public string? SN { get; set; }
    public string? RN { get; set; }
    public string? Email { get; set; }
    public string? Username { get; set; }
    public string? Password { get; set; }
    
}