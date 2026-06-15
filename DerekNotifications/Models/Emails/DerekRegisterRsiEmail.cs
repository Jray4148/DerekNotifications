using DerekNotifications.Models.Requests;

namespace DerekNotifications.Models.Emails;

public class DerekRegisterRsiEmail : Email<DerekRegisterRequest>
{
    public DerekRegisterRsiEmail(DerekRegisterRequest request, Services.Emails emails) : base(request)
    {
        To = emails.RegisterTo;
        From = emails.RegisterFrom; 
        Subject = "Registration - Derek";
        Body = $"""
                Company: {request.CoName}
                Serial Number: {request.SN}
                Registration Number: {request.RN}
                Email: {request.Email}
                Registration Key: {request.RegistrationKey}  <= Send this to the user

                ********************************************                

                Dear Roughneck Customer,
                 
                Thank you again for registering your software.  Here is your KEY to unlock your software:
                 {request.RegistrationKey}  

                Enter this key into 'Step 2' in the Derek Registration Process. 

                If you have any questions, please let us know, our team is here to help. 

                 
                Thank you,
                Roughneck Systems, Inc.
                972-552-5205
                sales@roughnecksoftware.com
                """;        
    }
}