using DerekNotifications.Models.Requests;

namespace DerekNotifications.Models.Issues;

public class EFileUnderstandIssueYouTrack : Issue
{
        public EFileUnderstandIssueYouTrack(EFileUnderstandRequest request, string projectId) 
        : base(request)
    {
        Project = new ProjectYouTrack { Id = projectId };
        Summary = $"I understand - { request.SN }";
        CustomFields =
        [
            new CustomField
            {
                Name = "Company",
                Type = "SimpleIssueCustomField",
                Value = request.CoName ?? ""
            },
            new CustomField
            {
                Name = "Serial Number",
                Type = "SimpleIssueCustomField",
                Value = request.SN ?? ""
            },
            new CustomField
            {
                Name = "Add1",
                Type = "SimpleIssueCustomField",
                Value = request.Add1 ?? ""
            },
            new CustomField
            {
                Name = "Add2",
                Type = "SimpleIssueCustomField",
                Value = request.Add2 ?? ""
            },
            new CustomField
            {
                Name = "City",
                Type = "SimpleIssueCustomField",
                Value = request.City ?? ""
            },
            new CustomField
            {
                Name = "St",
                Type = "SimpleIssueCustomField",
                Value = request.St ?? ""
            },
            new CustomField
            {
                Name = "Contact",
                Type = "SimpleIssueCustomField",
                Value = request.Contact ?? ""
            },
            new CustomField
            {
                Name = "Phone",
                Type = "SimpleIssueCustomField",
                Value = request.Phone ?? ""
            },
            new CustomField
            {
                Name = "Initials",
                Type = "SimpleIssueCustomField",
                Value = request.Initials ?? ""
            },
        ];
        Description = $"""
                       I understand - { request.SN } 
                       
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