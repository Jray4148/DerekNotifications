using DerekNotifications.Models.Requests;

namespace DerekNotifications.Models.Emails;

public class EFileUnderstandEmail : Email<EFileUnderstandRequest>
{
    public EFileUnderstandEmail(EFileUnderstandRequest request, Services.Emails emails)
        : base(request)
    {
        To = emails.EFileUnderstandTo; 
        From = emails.EFileFrom;   
        Subject = $"I understand - { request.SN } ";
        Body = $"""
                Company: {request.CoName}
                Serial Number: {request.SN}
                Add1: {request.Add1}
                Add2: {request.Add2}
                City: {request.City}
                State: {request.St}
                Contact: {request.Contact}
                Phone: {request.Phone}
                Initials: {request.Initials}
                """;
    }
}
