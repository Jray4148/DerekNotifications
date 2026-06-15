using DerekNotifications.Models.Requests;

namespace DerekNotifications.Models.Emails;

public class EFileBillMeEmail : Email<EFileBillMeRequest>
{
    public EFileBillMeEmail(EFileBillMeRequest request, Services.Emails emails)
        : base(request)
    {
        To = emails.EFileBillMeTo; 
        From = emails.EFileFrom;   
        Subject = $"{ request.Action } SN: { request.SN } BOID: {request.BOID}";
        Body = $"""
                Company: {request.CoName}
                Serial Number: {request.SN}
                Batch OID: {request.BOID}
                Cost: {request.Cost}
                Email: {request.Email}
                UserID: {request.UserID}
                FormType: {request.FormType}
                Quantity: {request.Quantity}
                Gross: {request.Gross}
                Tax: {request.Tax}
                """;
    }
}
