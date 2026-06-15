using System.ComponentModel.DataAnnotations;

namespace DerekNotifications.Models.Requests;

public class InvoicesCreateRequest
{
    [Required]
    public required List<string> TicketIds { get; set; }
}