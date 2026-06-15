using DerekNotifications.Models;
using DerekNotifications.Services;

namespace DerekNotifications.Tests.Services;

public class ZohoInvoicesServiceTestsBuildInvoiceFromTickets
{
    [Fact]
    public void ReturnsNull_WhenTicketsListIsEmpty()
    {
        // Arrange
        var tickets = new List<BillMeTicket>();

        // Act
        var result = InvoicesServiceZoho.BuildInvoiceFromTickets(tickets);

        // Assert
        Assert.Null(result);
    }
    
    [Fact]
    public void ReturnsInvoice_WithCorrectData_FromSingleTicket()
    {
        // Arrange
        var ticket = new BillMeTicket
        {
            Id = "-1",
            SerialNumber = "SN1001",
            Company = "Acme Corp",
            FormType = "F1",
            BatchId = "Batch1",
            Cost = 150.00m
        };
        var tickets = new List<BillMeTicket> { ticket };

        // Act
        var result = InvoicesServiceZoho.BuildInvoiceFromTickets(tickets);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("-1", result!.CustomerId);
        Assert.Equal("SN1001", result.RsiSerialNumber);
        Assert.Equal("Net 10", result.PaymentTermsLabel);
        Assert.Equal(10, result.PaymentTerms);
        
        Assert.NotNull(result.LineItems);
        Assert.Single(result.LineItems!);
        var item = result.LineItems![0];
        Assert.Equal("Batch ID # Batch1", item.Name);
        Assert.Equal("F1", item.FormType);
        Assert.Equal(150.00m, item.Rate);
    }
    
    [Fact]
    public void ReturnsInvoice_AggregatingCost_FromMultipleTickets()
    {
        // Arrange
        var tickets = new List<BillMeTicket>
        {
            new() { Id = "-1", SerialNumber = "SN1001", Company = "Acme Corp", Cost = 100.00m, FormType = "F1", BatchId = "Batch1"  },
            new() { Id = "-1", SerialNumber = "SN1002", Company = "Acme Corp", Cost = 50.00m, FormType = "F2", BatchId = "Batch2" },
            new() { Id = "-1", SerialNumber = "SN1003", Company = "Acme Corp", Cost = 25.50m, FormType = "F3", BatchId = "Batch3" }
        };

        // Act
        var result = InvoicesServiceZoho.BuildInvoiceFromTickets(tickets);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("SN1001", result!.RsiSerialNumber); // Uses first ticket's SN
        
        Assert.NotNull(result.LineItems);
        Assert.Equal(3, result.LineItems.Count);
        
        Assert.Equal("Batch ID # Batch1", result.LineItems![0].Name);
        Assert.Equal(100.00m, result.LineItems![0].Rate); 
        Assert.Equal("F1", result.LineItems![0].FormType);
        
        Assert.Equal("Batch ID # Batch2", result.LineItems![1].Name);
        Assert.Equal(50.00m, result.LineItems![1].Rate); 
        Assert.Equal("F2", result.LineItems![1].FormType);

        Assert.Equal("Batch ID # Batch3", result.LineItems![2].Name);
        Assert.Equal(25.50m, result.LineItems![2].Rate); 
        Assert.Equal("F3", result.LineItems![2].FormType);
    }
    
}