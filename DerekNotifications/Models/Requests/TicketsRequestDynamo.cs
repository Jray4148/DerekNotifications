using System.ComponentModel.DataAnnotations;

namespace DerekNotifications.Models.Requests;

public class TicketsRequestDynamo
{
    [Required]
    public required string FormType { get; set; }
    [Required]
    public int Quantity { get; set; }
    public DateOnly Date { get; set; }
    public string? BatchOid { get; set; }
}