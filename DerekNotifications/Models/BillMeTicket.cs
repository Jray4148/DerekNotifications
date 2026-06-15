
namespace DerekNotifications.Models;

public enum RsiTicketType
{
    Unknown,
    BillMe,     
    BillMeConfirmed   
}

public class BillMeTicket
{
    public required string Id { get; set; }
    public string? Company { get; set; }
    public string? SerialNumber { get; set; }
    public string? Email { get; set; }
    public string? Summary { get; set; }
    public string? BatchId { get; set; }
    public string? FormType { get; set; }
    public decimal Cost { get; set; }
    public long Date { get; set; }
    public RsiTicketType TicketType { get; set; }
    public bool HasConfirmation { get; set; }
    
    public static BillMeTicket? Map(BillMeTicketYouTrack? source)
    {
        if (source == null) return null;
        
        var ticket = new BillMeTicket
        {
            Id = source.IdReadable,
            Company = source.Company,
            SerialNumber = source.SerialNumber?.ToLower(),
            Email = source.Email?.ToLower(),
            Summary = source.Summary ?? string.Empty,
            BatchId = source.BatchId,
            Cost = source.Cost,
            FormType = source.FormType,
            Date = source.Created,
            HasConfirmation = source.HasConfirmation
        };
        
        if (ticket.Summary.ToLower().Contains("bill_me_confirmed"))
        {
            ticket.TicketType = RsiTicketType.BillMeConfirmed;
        }
        else if (ticket.Summary.ToLower().Contains("bill_me"))
        {
            ticket.TicketType = RsiTicketType.BillMe;
        }
        else
        {
            ticket.TicketType = RsiTicketType.Unknown;
        }
        
        return ticket;
    }
}