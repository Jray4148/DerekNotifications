using CsvHelper.Configuration;

namespace DerekNotifications.Models;

public class NelcoInvoice
{
    public required string InvoiceNumber { get; set; }
    public required string OrderNumber { get; set; }
    public required string SoldTo { get; set; }
    public required string SoldToName { get; set; }
    public required string ItemNumber { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal ExtendedAmount { get; set; }
    public required string InvoiceDate { get; set; }
    public required string BatchNumber { get; set; }
    
    // Fields we add
    public required string Company { get; set; }
    public required decimal Gross { get; set; }
    public required decimal Cost { get; set; }
}

public sealed class NelcoInvoiceMap : ClassMap<NelcoInvoice>
{
    public NelcoInvoiceMap()
    {
        Map(m => m.InvoiceNumber).Name("Invoice Number");
        Map(m => m.OrderNumber).Name("Order Number");
        Map(m => m.SoldTo).Name("Sold To");
        Map(m => m.SoldToName).Name("Sold To Name");
        Map(m => m.ItemNumber).Name("Item Number");
        Map(m => m.Quantity).Name("Quantity");
        Map(m => m.UnitPrice).Name("Unit Price");
        Map(m => m.ExtendedAmount).Name("Extended Amount");
        Map(m => m.InvoiceDate).Name("Invoice Date");
        Map(m => m.BatchNumber).Name("Batch Number");
        Map(m => m.Company);
        Map(m => m.Gross);
        Map(m => m.Cost);
    }
}