using DerekNotifications.Models;
using DerekNotifications.Models.Requests;

namespace DerekNotifications.Interfaces;

public interface ITicketsService
{
    Task<List<BillMeTicket>> GetBillMeTicketsAsync(PagedRequest pagedRequest);
    Task<List<BillMeTicketYouTrack>> GetTicketsForIdsAsync(List<string> TicketIds);
    Task UpdateStateAsync(string issueId, string state);
    Task CreateAsync(Request request);
    Task UpdateBillMeTicketsConfirmationForBatchOidAsync(string batchOid);
    Task UpdateStateForDanglingBillMeTicketsForBatchOidAsync(string? batchOid);
    Task<List<BillMeTicket>> HasConfirmationCheck();
    Task<long> GetTicketCount();
}
