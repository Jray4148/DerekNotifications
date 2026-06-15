using DerekNotifications.Interfaces;
using DerekNotifications.Models;
using DerekNotifications.Models.Responses;

// https://www.zoho.com/invoice/api/v3/contacts/#list-contacts

namespace DerekNotifications.Services;

public class ContactsServiceZoho(
    HttpClient httpClient,
    ILogger<ContactsServiceZoho> logger) : IContactsService
{
    public async Task<Contact?> GetAsync(string? serialNumber)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(serialNumber))
            {
                logger.LogWarning("Serial Number cannot be null");
                return null;
            }
            
            var contact = await SubmitGetAsync(serialNumber);
            if (contact != null)
            {
                logger.LogInformation("Contact ID: {ContactId}", contact.Id);
            }

            return Contact.Map(contact);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error fetching contact for serial number {SerialNumber}", serialNumber);
            return null;
        }        
    }
    
    private async Task<ContactZoho?> SubmitGetAsync(string serialNumber)
    {
        var query = await new FormUrlEncodedContent(new Dictionary<string, string>
        {
            { "cf_serial_number", serialNumber },
        }).ReadAsStringAsync();

        var uriBuilder = new UriBuilder(httpClient.BaseAddress!)
        {
            Query = query
        };

        var result = await httpClient.GetFromJsonAsync<ZohoContactsResponse>(uriBuilder.Uri);
        return result?.Contacts?.FirstOrDefault();
    }    
}
