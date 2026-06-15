using DerekNotifications.Models.Requests;

namespace DerekNotifications.Models.Emails;

public class EbsRegisterRsiEmail : Email<EbsRegisterRequest>
{
    public EbsRegisterRsiEmail(EbsRegisterRequest request, Services.Emails emails)
        : base(request)
    {
        To = emails.RegisterTo; 
        From = emails.RegisterFrom; 
        Subject = "Registration - EBS";
        Body = $"""
                Company: {request.CoName}
                Serial Number: {request.SN}
                Registration Number: {request.RN}
                Email: {request.Email}
                Username: {request.Username}
                Password: {request.Password}
                """;
    }
}