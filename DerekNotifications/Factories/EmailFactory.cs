using DerekNotifications.Interfaces;
using DerekNotifications.Models.Emails;
using DerekNotifications.Models.Requests;
using DerekNotifications.Services;
using Microsoft.Extensions.Options;

namespace DerekNotifications.Factories;

public class EmailFactory(IOptions<AppSettingsService> settings) : IEmailFactory
{
    public List<Email> Create(Request request)
    {
        var emails = settings.Value.Rsi.Emails;
        
        return request.Type switch
        {
            Constants.RequestType.DerekRegister => [
                new DerekRegisterRsiEmail((DerekRegisterRequest)request, emails),
                new DerekRegisterClientEmail((DerekRegisterRequest)request, emails)
            ],
            Constants.RequestType.EbsRegister => [
                new EbsRegisterRsiEmail((EbsRegisterRequest)request, emails),
                new EbsRegisterClientEmail((EbsRegisterRequest)request, emails)
            ],
            Constants.RequestType.EFileUnderstand => [
                new EFileUnderstandEmail((EFileUnderstandRequest)request, emails),
            ],
            Constants.RequestType.EFileBillMe => [
                new EFileBillMeEmail((EFileBillMeRequest)request, emails),
            ],
            Constants.RequestType.Unknown => throw new ArgumentException("RequestType"),
            _ => throw new ArgumentOutOfRangeException()
        };
    }
    
}