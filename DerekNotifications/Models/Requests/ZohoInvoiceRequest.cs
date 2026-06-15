using System.Text.Json.Serialization;

namespace DerekNotifications.Models.Requests;

public class ZohoInvoiceRequest : Request
{
    public string? Company { get; set; }
    public string? SerialNumber { get; set; }
    [JsonPropertyName("Batch OID")]
    public string? BatchId { get; set; }
    public decimal? Cost { get; set; }
    public string? Email { get; set; }
    public string? UserId { get; set; }
    public string? FromType { get; set; }
    public decimal? Quantity { get; set; }
    public decimal? Gross { get; set; }
    public decimal? Tax { get; set; }
}