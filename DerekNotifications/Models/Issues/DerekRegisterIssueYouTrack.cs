using DerekNotifications.Models.Requests;

namespace DerekNotifications.Models.Issues;

public class DerekRegisterIssueYouTrack : Issue
{
    public DerekRegisterIssueYouTrack(DerekRegisterRequest request) 
        : base(request)
    {
        Project = new ProjectYouTrack { Id = Constants.Yt.RegisterProjectId };
        Summary = "Registration - Derek";
        CustomFields =
        [
            new CustomField
            {
                Name = "Product",
                Type = "SingleEnumIssueCustomField",
                Value = new EnumValue { Type = "EnumBundleElement", Name = Constants.Products.DerekShort }
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
            }
        ];
        Description = $"""
                       Company: { request.CoName }
                       Serial Number: { request.SN }
                       Registration Number: { request.RN }
                       Email: { request.Email }
                       Registration Key: {request.RegistrationKey}  <= Send this to the user

                       **************************
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
