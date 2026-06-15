using DerekNotifications.Models.Requests;

namespace DerekNotifications.Models.Issues;

public class EbsRegisterIssueYouTrack : Issue
{
    public EbsRegisterIssueYouTrack(EbsRegisterRequest request)
        : base(request)
    {
        Project = new ProjectYouTrack { Id = Constants.Yt.RegisterProjectId };
        Summary = "Registration - EBS";
        CustomFields =
        [
            new CustomField
            {
                Name = "Product",
                Type = "SingleEnumIssueCustomField",
                Value = new EnumValue { Type = "EnumBundleElement", Name = Constants.Products.EbsShort }
            },
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
                Name = "Registration Number",
                Type = "SimpleIssueCustomField",
                Value = request.RN ?? ""
            },
            new CustomField
            {
                Name = "Email",
                Type = "SimpleIssueCustomField",
                Value = request.Email ?? ""
            },
            new CustomField
            {
                Name = "Username",
                Type = "SimpleIssueCustomField",
                Value = request.Username ?? ""
            },
            new CustomField
            {
                Name = "Password",
                Type = "SimpleIssueCustomField",
                Value = request.Password ?? ""
            }
        ];
        Description = $"""
                       Company: { request.CoName }
                       Serial Number: { request.SN }
                       Registration Number: { request.RN }
                       Email: { request.Email }
                       Username: { request.Username }
                       Password: { request.Password }
                       """;
    }
}
