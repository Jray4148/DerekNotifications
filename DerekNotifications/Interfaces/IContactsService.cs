using DerekNotifications.Models;

namespace DerekNotifications.Interfaces;

public interface IContactsService
{
    Task<Contact?> GetAsync(string? serialNumber);
}
