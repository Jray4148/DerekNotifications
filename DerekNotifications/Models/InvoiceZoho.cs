namespace DerekNotifications.Models;

public class InvoiceZoho
{
    public string? customer_id { get; set; }
    public string? payment_terms_label { get; set; }
    public int? payment_terms { get; set; }
    public string? notes { get; set; }
    public string? invoice_number { get; set; }
    public List<InvoiceLineItemsZoho>? line_items { get; set; }
    public PaymentOptionsContainerZoho? payment_options { get; set; }
    
    public static InvoiceZoho? Map(Invoice? source)
    {
        if (source == null) return null;

        return new InvoiceZoho
        {
            customer_id = source.CustomerId,
            invoice_number = source.InvoiceNumber, 
            payment_terms_label = source.PaymentTermsLabel,
            payment_terms = source.PaymentTerms,
            notes = source.Notes,
            line_items = source.LineItems?.Select(i => new InvoiceLineItemsZoho
            {
                name = i.Name,
                rate = i.Rate,
                quantity = i.Quantity,
                tax_exemption_id = i.TaxExemption 
            }).ToList(),
            payment_options = new PaymentOptionsContainerZoho
            {
                payment_gateways = source.PaymentOptions?.Select(x => new PaymentOptionsZoho
                {
                    configured = x.Configured,
                    gateway_name = x.GatewayName
                }).ToList()
            }
        };
    }    
}

public class InvoiceLineItemsZoho
{
    public string? name { get; set; }
    public decimal? rate { get; set; }
    public int? quantity { get; set; }
    public string? tax_exemption_id { get; set; }
}

public class PaymentOptionsZoho
{
    public bool? configured { get; set; }
    public string? gateway_name { get; set; }
}

public class PaymentOptionsContainerZoho
{
    public List<PaymentOptionsZoho>? payment_gateways { get; set; }
}