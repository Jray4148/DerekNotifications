using DerekNotifications.Models.Requests;

namespace DerekNotifications.Interfaces;

public interface ICsvService
{
    Task JoinNelcoAndRsiInvoiceData(NelcoCsvRequest request);
}