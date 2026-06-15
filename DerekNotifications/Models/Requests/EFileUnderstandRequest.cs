namespace DerekNotifications.Models.Requests;

public class EFileUnderstandRequest : Request
{
    public string? CoName { get; set; }
    public string? SN { get; set; }
    public string? Add1 { get; set; }
    public string? Add2 { get; set; }
    public string? City { get; set; }
    public string? St { get; set; }
    public string? Contact { get; set; }
    public string? Phone { get; set; }
    public string? Initials { get; set; }
}