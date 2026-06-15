using DerekNotifications.Models.Requests;

namespace DerekNotifications.Models.Issues;

public class AskDerekIssueYouTrack : Issue
{
    public AskDerekIssueYouTrack(AskDerekRequest request) 
        : base(request)
    {
        Project = new ProjectYouTrack { Id = Constants.Yt.AskDerekProjectId };
        Summary = request.Question;
        Description = request.Answer;
        CustomFields = [
            new CustomField
            {
                Name = "AddedBy",
                Type = "SimpleIssueCustomField",
                Value = request.AskedBy ?? ""
            }
        ];
    }
}
