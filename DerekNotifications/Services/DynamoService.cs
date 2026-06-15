using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using DerekNotifications.Interfaces;
using DerekNotifications.Models;
using DerekNotifications.Models.Requests;

namespace DerekNotifications.Services;

public class DynamoService(
    IDynamoDBContext context,
    ILogger<DynamoService> logger) : IDynamoService
{
    public async Task CreateAsync(EFileBillMeRequest request)
    {
        try
        {
            var invoice = new BillMeTicketDynamo
            {
                Id = Guid.NewGuid().ToString(),
                Company = request.CoName,
                Action = request.Action?.ToUpper(),
                SerialNumber = request.SN,
                BatchOid = request.BOID,
                Cost = request.Cost,
                Email = request.Email,
                UserId = request.UserID,
                FormType = request.FormType,
                Quantity = request.Quantity,
                Gross = request.Gross,
                Tax = request.Tax
            };

            await context.SaveAsync(invoice);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to create Dynamo Ticket, Request: {@Request}", request);
        }
    }

    public async Task<BillMeTicketDynamo?> GetByBatchOidAsync(string batchOid)
    {
        if (string.IsNullOrWhiteSpace(batchOid)) return null;

        var queryConfig = new QueryOperationConfig
        {
            IndexName = "BatchOid",
            Filter = new QueryFilter(nameof(BillMeTicketDynamo.BatchOid), QueryOperator.Equal, batchOid)
        };

        var search = context.FromQueryAsync<BillMeTicketDynamo>(queryConfig);
        var results = await search.GetRemainingAsync();
        
        var confirmed = results
            .Where(x => string.Equals(x.Action, "bill_me_confirmed", StringComparison.OrdinalIgnoreCase))
            .ToList().FirstOrDefault();
        
        return confirmed;
    }    
    /*
    public async Task<List<BillMeTicketDynamo>> GetByFormTypeForDayUtcAsync(TicketsRequestDynamo request)
    {
        
        var startUtc = request.Date.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);
        var endInclusiveUtc = startUtc.AddDays(1).AddTicks(-1);

        var config = new QueryConfig
        {
            IndexName = "FormTypeCreatedAt"
        };

        var search = context.QueryAsync<BillMeTicketDynamo>(
            request.FormType,
            QueryOperator.Between,
            [startUtc, endInclusiveUtc],
            config);

        var dynamos = await search.GetRemainingAsync();
        
        const string billMeConfirmedAction = "bill_me_confirmed";

        var confirmed = dynamos
            .Where(x => string.Equals(x.Action, billMeConfirmedAction, StringComparison.OrdinalIgnoreCase))
            .ToList();

        if (request.Quantity > 0)
        {
            return confirmed.Where(x => x.Quantity == request.Quantity).ToList();
        }

        return confirmed;
    }    
    
    public async Task<List<BillMeTicketDynamo>> GetBillMeTicketsAsync(TicketsRequestDynamo request)
    {
        var queryConfig = new QueryConfig
        {
            IndexName = "FormTypeQuantity"
        };

        var search = context.QueryAsync<BillMeTicketDynamo>(
            request.FormType,
            QueryOperator.Equal,
            [request.Quantity],
            queryConfig);

        return await search.GetRemainingAsync();
    }    
    
    public async Task<List<BillMeTicketDynamo>> GetBillMeTicketsAsync(string actionValue)
    {
        var queryConfig = new QueryOperationConfig
        {
            IndexName = "Action-Index", 
            Filter = new QueryFilter("Action", QueryOperator.Equal, actionValue)
        };

        var search = context.FromQueryAsync<BillMeTicketDynamo>(queryConfig);
        return await search.GetRemainingAsync();
    }    
    */
}