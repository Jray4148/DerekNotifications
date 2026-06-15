using DerekNotifications.Models;

namespace DerekNotifications.Interfaces;

public interface IInvoicesServices
{
    Task CreateInvoicesForSerialNumber(List<BillMeTicket> tickets, List<string>? ticketIds);
    Task<BuildInvoicesResult> BuildInvoicesForSerialNumber(List<BillMeTicket> tickets);
}
