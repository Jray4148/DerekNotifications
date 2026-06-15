using DerekNotifications.Models;
using DerekNotifications.Models.Requests;

namespace DerekNotifications.Services;

public partial class TicketsServiceYouTrack
{
    /// <summary>
    /// Retrieves a list of BillMe tickets from YouTrack based on the provided filter criteria.  This will return
    /// tickets in state "To do" and summary containing "BILL_ME", which means it will get both BILL_ME and BILL_ME_CONFIRMED tickets.
    /// </summary>
    /// <param name="request">The request object containing the filter criteria for the BillMe tickets.</param>
    /// <param name="pagedRequest">the request object containing the page criteria for the pagination</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a list of BillMe tickets that match the filter criteria.</returns>
    public async Task<List<BillMeTicket>> GetBillMeTicketsAsync(PagedRequest pagedRequest)
    {
        const string responseFields = "idReadable,created,summary,project(name),customFields(name,value(name))";
        var searchExpression = $"project:{appSettingsService.Value.Yt.EFile25ProjectName} State:{{To Do}} summary: BILL_ME";
        var tickets = await GetTicketsAsync(searchExpression, responseFields , pagedRequest.TicketCount);
        return FilterBillMeTickets(tickets); 
    }
    
    /// <summary>
    /// Updates the confirmation status of corresponding BillMe tickets in YouTrack based on the provided request.
    /// </summary>
    /// <param name="batchOid">The request object containing details necessary for identifying and updating the corresponding BillMe tickets.</param>
    /// <returns>A task that represents the asynchronous operation of updating the confirmation status of the tickets.</returns>
    public async Task UpdateBillMeTicketsConfirmationForBatchOidAsync(string batchOid)
    {
        List<BillMeTicketYouTrack> tickets = [];
        
        for (var attempt = 0; attempt < 5; attempt++)
        {
            tickets = await GetYouTrackBillMeTicketsForBatchOidAsync(batchOid);
            if (tickets.Count > 0) break;
             await Task.Delay(5000);
        }
        
        if (tickets.Count == 0)
        {
            logger.LogInformation("No tickets found for batch oid: {BatchOid}", batchOid);
            return;
        }
        
        var payload = CreateIssueUpdatePayload("HasConfirmation", 1);
        
        foreach (var ticket in tickets)
        {
            await UpdateIssueAsync(ticket.IdReadable, payload);
            logger.LogInformation("Updated confirmation status for ticket: {TicketId}", ticket.IdReadable);
        }
    }

    public async Task UpdateStateForDanglingBillMeTicketsForBatchOidAsync(string? batchOid)
    {
        if (batchOid == null) return;
        var tickets = await GetYouTrackBillMeTicketsForBatchOidAsync(batchOid);
        
        foreach (var ticket in tickets)
        {
            await UpdateStateAsync(ticket.IdReadable, "Done");
            logger.LogInformation("Updated confirmation status for ticket: {TicketId}", ticket.IdReadable);
        }
    }
 
    private async Task<List<BillMeTicketYouTrack>> GetYouTrackBillMeTicketsForBatchOidAsync(string batchOid)
    {
        var searchExpression = $"project:{appSettingsService.Value.Yt.EFile25ProjectName} State:{{To Do}} Batch OID: {batchOid} summary: BILL_ME -";
        var tickets = await GetTicketsAsync(searchExpression, "idReadable,summary,customFields(name,value(name))");
        var onlyBillMeTickets = tickets.Where(t => t.Summary != null && t.Summary.Contains("BILL_ME -")).ToList();
        return onlyBillMeTickets;
    }
    
    public async Task<List<BillMeTicketYouTrack>> GetTicketsForIdsAsync(List<string> TicketIds)
    {
        const string responseFields = "idReadable,created,summary,project(name),customFields(name,value(name))";
        var searchExpression = string.Join(" ", TicketIds);
        
        try
        {
            var uri = await BuildSearchRequestUri(searchExpression, responseFields);
            return await httpClient.GetFromJsonAsync<List<BillMeTicketYouTrack>>(uri) ?? [];
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error fetching tickets for : {search}", searchExpression);
            return [];
        }
    }    

    
    private async Task<List<BillMeTicketYouTrack>> GetTicketsAsync(string searchExpression, string responseFields, string? top = null)
    {
        try
        {
            var uri = await BuildSearchRequestUri(searchExpression, responseFields);
            return await httpClient.GetFromJsonAsync<List<BillMeTicketYouTrack>>(uri) ?? [];
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error fetching tickets for : {search}", searchExpression);
            return [];
        }
    }    
    
    private static List<BillMeTicket> FilterBillMeTickets(List<BillMeTicketYouTrack> tickets)
    {
        var result = tickets.Select(BillMeTicket.Map)
            .OfType<BillMeTicket>()
            .ToList();
        
        return result;
    }
    
    public async Task<List<BillMeTicket>> HasConfirmationCheck()
    {
        var searchExpression = $"project:{appSettingsService.Value.Yt.EFile25ProjectName} State:{{To Do}} summary: BILL_ME_CONFIRMED";
        var confirmedYouTrackTickets = await GetTicketsAsync(searchExpression, "idReadable,summary,customFields(name,value(name))");
        var confirmedTickets = confirmedYouTrackTickets.Select(BillMeTicket.Map).OfType<BillMeTicket>().ToList();
        
        foreach (var ticket in confirmedTickets)
        {
            if (ticket.BatchId == null) continue;
            await UpdateBillMeTicketsConfirmationForBatchOidAsync(ticket.BatchId);
        }
        
        return confirmedTickets;
    }
    
    public async Task<long> GetTicketCount()
    {
        // "Cost:*" is used to filter tickets without a cost value, this filters out the "I understand" tickets"
        var query = $"project:{appSettingsService.Value.Yt.EFile25ProjectName} State:{{To Do}} Cost:*";
        const string uri = "/api/issuesGetter/count?fields=count";
        for (var attempt = 0; attempt < 10; attempt++)
        {
            var response = await httpClient.PostAsJsonAsync(uri, new { query });
            response.EnsureSuccessStatusCode();

            var body = await response.Content.ReadFromJsonAsync<IssueCountResponse>();
            
            if (body?.Count == -1)
            {
                await Task.Delay(200);
                continue;
            }
            
            return body?.Count ?? 0;
        }
        return 0;
    }
    
    private sealed class IssueCountResponse
    {
        public long Count { get; set; }
    }
}