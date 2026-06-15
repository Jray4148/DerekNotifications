using DerekNotifications.Models;
using DerekNotifications.Models.Requests;

namespace DerekNotifications.Interfaces;

public interface IDynamoService
{
    Task CreateAsync(EFileBillMeRequest request);
    Task<BillMeTicketDynamo?> GetByBatchOidAsync(string batchOid);
    
}