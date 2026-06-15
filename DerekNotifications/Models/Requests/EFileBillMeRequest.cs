namespace DerekNotifications.Models.Requests;

public class EFileBillMeRequest : Request
{
    public string? CoName { get; set; }
    public string? Action { get; set; }
    public string? SN { get; set; }
    public string? BOID { get; set; }
    public decimal? Cost { get; set; }
    public string? Email { get; set; }
    public string? UserID { get; set; }
    public string? FormType { get; set; }
    public int? Quantity { get; set; }
    public decimal? Gross { get; set; }
    public decimal? Tax { get; set; }
}