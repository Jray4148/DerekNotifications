using DerekNotifications.Models.Requests;

namespace DerekNotifications.Models.Issues;

public class EFileBillMeIssueYouTrack : Issue
{
    public EFileBillMeIssueYouTrack(EFileBillMeRequest request, string projectId)
        : base(request)
    {
        Project = new ProjectYouTrack { Id = projectId };
        Summary = $"{ request.Action } - { request.SN } ";
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
                Name = "Batch OID",
                Type = "SimpleIssueCustomField",
                Value = request.BOID ?? ""
            },
            new CustomField
            {
                Name = "Cost",
                Type = "SimpleIssueCustomField",
                Value = request.Cost
            },
            new CustomField
            {
                Name = "Email",
                Type = "SimpleIssueCustomField",
                Value = request.Email ?? ""
            },
            new CustomField
            {
                Name = "UserID",
                Type = "SimpleIssueCustomField",
                Value = request.UserID ?? ""
            },
            new CustomField
            {
                Name = "FormType",
                Type = "SimpleIssueCustomField",
                Value = request.FormType ?? ""
            },
            new CustomField
            {
                Name = "Quantity",
                Type = "SimpleIssueCustomField",
                Value = request.Quantity
            },
            new CustomField
            {
                Name = "Gross",
                Type = "SimpleIssueCustomField",
                Value = request.Gross
            },
            new CustomField
            {
                Name = "Tax",
                Type = "SimpleIssueCustomField",
                Value = request.Tax
            },
            new CustomField
            {
                Name = "HasConfirmation",
                Type = "SimpleIssueCustomField",
                Value = request.Action!.Contains("BILL_ME_CONFIRMED", StringComparison.OrdinalIgnoreCase) ? 1 : 0
            },
        ];
          Description = $"""
                         { request.Action } - { request.SN }
                         
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


