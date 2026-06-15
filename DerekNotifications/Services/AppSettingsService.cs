namespace DerekNotifications.Services;

public class AppSettingsService
{
    public required Rsi Rsi { get; set; }
    public required Yt Yt { get; set; }
    public required Zoho Zoho { get; set; }
}

public class Rsi
{
    public required string ApiToken { get; set; }
    public required Emails Emails { get; set; }
}

public class Emails
{
    public required string RegisterFrom { get; set; }
    public required string RegisterTo { get; set; }
    public required string EFileFrom { get; set; }
    public required string EFileUnderstandTo { get; set; }
    public required string EFileBillMeTo { get; set; }
}


public class Yt
{
    public required string ApiToken { get; set; }
    public required string EFile25ProjectId { get; set; }
    public required string EFile25ProjectName { get; set; }
}

public class Zoho
{
    public required string RefreshToken { get; set; }
    public required string ClientId { get; set; }
    public required string ClientSecret { get; set; }
}