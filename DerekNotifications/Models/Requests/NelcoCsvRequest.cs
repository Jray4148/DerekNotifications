using System.ComponentModel.DataAnnotations;

namespace DerekNotifications.Models.Requests;

public class NelcoCsvRequest
{
    [Required]
    public required string Filename { get; set; }
}