namespace DerekNotifications;

public abstract class Constants
{
    public const string CorsPolicyName = "_CorsPolicyName";
    
    public static class Products
    {
        public const string Derek = "Derek";
        public const string DerekShort = "Derek";
        public const string Ebs = "Roughneck Emergency Backup";
        public const string EbsShort = "EBS";
    }
    
    public enum RequestType
    {
        Unknown,
        DerekRegister,
        EbsRegister,
        EFileUnderstand,
        EFileBillMe,
        AskDerek
    }

    public static class Rsi
    {
        public const string ApiKeyHeaderName = "RSI-API-KEY";
    }
    
    public static class Yt
    {
        public const string RegisterProjectId = "0-21";
        public const string AskDerekProjectId = "0-25";
        public const string Url = "https://rsi.youtrack.cloud/api/issues/";
    }
    
    public static class Zoho
    {
        public const string OrganizationId = "730551939";
        public const string AuthUrl = "https://accounts.zoho.com/oauth/v2/token";
        public const string ContactsUrl = "https://www.zohoapis.com/invoice/v3/contacts";
        public const string InvoicesUrl = "https://www.zohoapis.com/invoice/v3/invoices";
    }
    
    public static class Invoice
    {
        public const string DefaultCustomerId = "-1";
        public const string DefaultPaymentTermsLabel = "Net 10";
        public const int DefaultPaymentTerms = 10;
        public const string DefaultNotes = $"""
                                            Please note our new address:
                                            510 Turtle Cove Blvd., Suite 100
                                            Rockwall, TX 75087
                                            
                                            Thanks for your business
                                            """;
    }
    
    public static class LessAnnoying
    {
       public const string Url = "https://api.lessannoyingcrm.com/v2/";
    }
}