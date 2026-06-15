using DerekNotifications.Models.Requests;

namespace DerekNotifications.Models;

public class Invoice
{
    public string? Company { get; set; }
    public string? CustomerId { get; set; }
    public string? RsiSerialNumber { get; set; }
    public string? Email { get; set; }
    public decimal? Cost { get; set; }
    public string? PaymentTermsLabel { get; set; }
    public int? PaymentTerms { get; set; }
    public string? Notes { get; set; }
    public List<InvoiceLineItems>? LineItems { get; set; }
    public string? InvoiceNumber { get; set; }
    public List<PaymentOptions>? PaymentOptions { get; set; }
    public string? ErrorMessage { get; set; }
}

public class InvoiceLineItems
{
    public string? Id { get; set; }
    public string? BatchId { get; set; }
    public string? Name { get; set; }
    public decimal? Rate { get; set; }
    public int? Quantity { get; set; }
    public string? FormType { get; set; }
    public string? TaxExemption { get; set; }

}

public class PaymentOptions
{
    public bool? Configured { get; set; }
    public string? AdditionalField1  { get; set; }
    public string? GatewayName  { get; set; }
}

public sealed record BuildInvoicesResult(
    List<Invoice> Invoices,
    List<Invoice> SkippedInvoices
);