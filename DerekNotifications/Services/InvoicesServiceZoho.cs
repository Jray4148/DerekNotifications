using DerekNotifications.Interfaces;
using DerekNotifications.Models;

namespace DerekNotifications.Services;

public class InvoicesServiceZoho(
    HttpClient httpClient,
    IContactsService contactsService,
    ITicketsService ticketsService,
    ILogger<InvoicesServiceZoho> logger) : IInvoicesServices
{
    /// <summary>
    /// Creates invoices for a specific serial number, processing tickets based on their IDs.
    /// Filters and processes a list of tickets that match the provided ticket IDs, then generates invoices
    /// for the specified serial number using those tickets.
    /// </summary>
    /// <param name="tickets">The list of <see cref="BillMeTicket"/> objects to be filtered and processed for invoice creation.</param>
    /// <param name="ticketIds">The list of ticket IDs used to filter the provided tickets. Cannot be null or empty.</param>
    /// <exception cref="ArgumentException">Thrown when the ticket IDs are null or an empty list.</exception>
    /// <returns>A task representing the asynchronous operation of creating invoices for the specified serial number.</returns>
    public async Task CreateInvoicesForSerialNumber(List<BillMeTicket> tickets, List<string>? ticketIds)
    {
        if (ticketIds == null || ticketIds.Count == 0)
        {
            throw new ArgumentException("Ticket IDs cannot be null or empty.", nameof(ticketIds));
        }
        
        tickets = tickets.Where(t => ticketIds.Contains(t.Id)).ToList();
        await CreateInvoicesForSerialNumber(tickets);
    }

    private async Task CreateInvoicesForSerialNumber(List<BillMeTicket> tickets)
    {
        var result = await BuildInvoicesForSerialNumber(tickets);
        foreach (var invoice in result.Invoices)
        {
            await ProcessInvoiceAsync(invoice);
        }
    }

    /// <summary>
    /// Builds a list of invoices from a collection of tickets grouped by serial number and email.
    /// Filters the tickets applicable for invoicing, groups them by serial number and email, and creates invoices
    /// with customer information and validation checks.
    /// </summary>
    /// <param name="tickets">The list of <see cref="BillMeTicket"/> objects to process for creating invoices.</param>
    /// <returns>A list of <see cref="Invoice"/> objects built from the provided tickets.</returns>
    public async Task<BuildInvoicesResult> BuildInvoicesForSerialNumber(List<BillMeTicket> tickets)
    {
        var invoiceTickets = GetInvoiceTickets(tickets);
        var invoices = new List<Invoice>();
        var skippedInvoices = new List<Invoice>();
        var groupedTickets = invoiceTickets.GroupBy(t => new { t.SerialNumber, t.Email });
        foreach (var group in groupedTickets)
        {
            var groupedInvoiceTickets = group.ToList();
            var invoice = BuildInvoiceFromTickets(groupedInvoiceTickets);
            
            var contact = await contactsService.GetAsync(group.Key.SerialNumber);
            invoice?.CustomerId = contact?.Id;
            
            var isValid = ValidateInvoice(invoice);
            
            if (!isValid)
            {
                if (invoice != null) skippedInvoices.Add(invoice);
                continue;
            }

            invoices.Add(invoice!);
        }
        return new BuildInvoicesResult(invoices, skippedInvoices);
    }

    private async Task ProcessInvoiceAsync(Invoice invoice)
    {
        var createdInvoice = await CreateAsync(invoice);

        if (createdInvoice?.LineItems is null)
            return;

        foreach (var lineItem in createdInvoice.LineItems)
        {
            if (lineItem.Id is not null)
            {
                // Move tickets used as line items to Done state
                await ticketsService.UpdateStateAsync(lineItem.Id, "Done");
                // Move "dangling" tickets.  These tickets were created but never confirmed, but have same BatchId
                await ticketsService.UpdateStateForDanglingBillMeTicketsForBatchOidAsync(lineItem.BatchId);
            }
        }
    }

    /// <summary>
    /// Get list of tickets that are used to create invoices.
    /// Filters a list of <see cref="BillMeTicket"/> to include only those that have a non-null summary
    /// containing the keyword "bill_me_confirmed" (case-insensitive).
    /// </summary>
    /// <param name="tickets">A list of <see cref="BillMeTicket"/> objects to be filtered.</param>
    /// <returns>A filtered list of <see cref="BillMeTicket"/> where the <c>Summary</c> property contains "bill_me_confirmed".</returns>
    private static List<BillMeTicket> GetInvoiceTickets(List<BillMeTicket> tickets)
    {
        return tickets
            .Where(x => x.Summary != null && x.Summary.ToLower().Contains("bill_me_confirmed"))
            .ToList();
    }
    
    private bool ValidateInvoice(Invoice? invoice)
    {
        if (invoice == null)
        {
            logger.LogWarning("Invoice creation skipped: Invoice is missing.");
            return false;
        }
        
        if (string.IsNullOrWhiteSpace(invoice.RsiSerialNumber)) 
        {
            invoice.ErrorMessage = "Invoice creation skipped: Serial Number is missing.";
            logger.LogWarning("{@ErrorMessage} Request: {@Invoice}", invoice.ErrorMessage, invoice);
            return false;
        }

        if (string.IsNullOrWhiteSpace(invoice.CustomerId))
        {
            invoice.ErrorMessage = "Invoice creation skipped: CustomerId is missing";
            logger.LogWarning("{@ErrorMessage} Request: {@Invoice}", invoice.ErrorMessage, invoice);
            return false;
        }
        
        return true;
    }

    private async Task<Invoice?> CreateAsync(Invoice invoice)
    {
        try
        {
            using var response = await httpClient.PostAsJsonAsync("", InvoiceZoho.Map(invoice));
            if (response.IsSuccessStatusCode) return invoice;
            var errorContent = await response.Content.ReadAsStringAsync();
            logger.LogError("Zoho API failed. Status: {StatusCode}, Reason: {ErrorContent}", response.StatusCode, errorContent);
            return null;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to create invoice for Customer Id: {@id}", invoice.CustomerId);
            return null;
        }
    }
    
    public static Invoice? BuildInvoiceFromTickets(List<BillMeTicket> tickets)
    {
        if (tickets.Count == 0) return null;
        
        var firstTicket = tickets.First();
        var cost = tickets.Sum(t => t.Cost);
        
        return new Invoice
        {
            Company = firstTicket.Company,
            RsiSerialNumber = firstTicket.SerialNumber,
            Email = firstTicket.Email,
            CustomerId = Constants.Invoice.DefaultCustomerId, // This will be replaced with the actual customer id
            PaymentTermsLabel = Constants.Invoice.DefaultPaymentTermsLabel,
            PaymentTerms = Constants.Invoice.DefaultPaymentTerms,
            Notes = $"""
                     {Constants.Invoice.DefaultNotes}
                     
                     Invoice emailed to: {firstTicket.Email}
                     """,
            Cost = cost,
            InvoiceNumber = firstTicket.Company?[..1] + cost.ToString().Replace(".", "") + "-" + firstTicket.BatchId,
            LineItems = tickets.Select(x => new InvoiceLineItems
            {
                Id = x.Id,
                BatchId = x.BatchId,
                Name = $"Batch ID # {x.BatchId}", 
                Rate = x.Cost,
                FormType = x.FormType,
                Quantity = 1,
                TaxExemption = "2427479000008684066" // Id for Efiling tax exemption
            }).ToList(),
            PaymentOptions = tickets.Select(x => new PaymentOptions
            {
                Configured = true,
                AdditionalField1 = "standard",
                GatewayName = "stripe"
            }).ToList()
        };
    }
}
