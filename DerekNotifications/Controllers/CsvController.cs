using DerekNotifications.Interfaces;
using DerekNotifications.Models.Requests;
using Microsoft.AspNetCore.Mvc;

namespace DerekNotifications.Controllers;

[Route("[controller]")]
public class CsvController(ICsvService csvService) : ControllerBase
{
    // Use Insomnia (DerekNotifications) /csv
    // Query:
    //      Filename name-of-file (do not include .csv)
    [HttpGet]
    public async Task<IActionResult> Join([FromQuery] NelcoCsvRequest request)
    {
        await csvService.JoinNelcoAndRsiInvoiceData(request);
        return Ok();
    }
 }