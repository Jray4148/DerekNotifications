using DerekNotifications.Interfaces;
using DerekNotifications.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DerekNotifications.Controllers;

[ApiController]
[Authorize]
[Route("[controller]")]
public class ContactsController(
    IContactsService contactsService) : ControllerBase
{
    [HttpGet("{serialNumber}")]
    public async Task<Contact?> Get([FromRoute] string serialNumber)
    {
        return await contactsService.GetAsync(serialNumber);
    }
}
