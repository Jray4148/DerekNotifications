using Amazon.DynamoDBv2.DataModel;

namespace DerekNotifications.Models;

[DynamoDBTable("BillMeTickets")]
public class BillMeTicketDynamo
{
    [DynamoDBHashKey]
    public required string Id { get; set; }
    public string? Company { get; set; }
    public string? Action { get; set; }
    public string? SerialNumber { get; set; }
    public string? BatchOid { get; set; }
    public decimal? Cost { get; set; }
    public string? Email { get; set; }
    public string? UserId { get; set; }
    public string? FormType { get; set; }
    public int? Quantity { get; set; }
    public decimal? Gross { get; set; }
    public decimal? Tax { get; set; }    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}