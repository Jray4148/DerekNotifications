using DerekNotifications.Interfaces;
using DerekNotifications.Models.Issues;
using DerekNotifications.Models.Requests;
using DerekNotifications.Services;
using Microsoft.Extensions.Options;

namespace DerekNotifications.Factories;

public class IssueFactoryYouTrack(IOptions<AppSettingsService> settings) : IIssueFactoryYouTrack
{
    public Issue Create(Request request)
    {
        var eFileProjectId = settings.Value.Yt.EFile25ProjectId;

        return request.Type switch
        {
            Constants.RequestType.DerekRegister => new DerekRegisterIssueYouTrack((DerekRegisterRequest)request),
            Constants.RequestType.EbsRegister => new EbsRegisterIssueYouTrack((EbsRegisterRequest)request),
            Constants.RequestType.AskDerek => new AskDerekIssueYouTrack((AskDerekRequest)request),
            Constants.RequestType.EFileUnderstand => new EFileUnderstandIssueYouTrack((EFileUnderstandRequest)request, eFileProjectId),
            Constants.RequestType.EFileBillMe => new EFileBillMeIssueYouTrack((EFileBillMeRequest)request, eFileProjectId),
            Constants.RequestType.Unknown => throw new NotImplementedException(),
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}