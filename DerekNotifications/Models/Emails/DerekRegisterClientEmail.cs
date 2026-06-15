using DerekNotifications.Models.Requests;

namespace DerekNotifications.Models.Emails;

public class DerekRegisterClientEmail : Email<DerekRegisterRequest>
{
    public DerekRegisterClientEmail(DerekRegisterRequest request, Services.Emails emails)
        : base(request)
    {
        To = request.Email!;
        From = emails.RegisterFrom;
        Subject = "Derek Registration";
        Body = $"""
                Dear Roughneck Customer,

                This is a confirmation email that your registration for { Constants.Products.Derek } is being 
                processed. If you do not receive a key to unlock your software within 48 hours, please e-mail 
                or give us a call.

                Thank you,
                RSI
                972-552-5204
                { emails.RegisterTo }
                """;
    }
}