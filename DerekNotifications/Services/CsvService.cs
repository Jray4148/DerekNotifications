using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using DerekNotifications.Interfaces;
using DerekNotifications.Models;
using DerekNotifications.Models.Requests;

namespace DerekNotifications.Services;

public class CsvService(IDynamoService dynamoService) : ICsvService
{
    /// <summary>
    /// Merges invoice data from a Nelco CSV file with additional data from BillMe tickets in DynamoDb that match the invoice batch numbers
    /// and generates updated output files.
    /// </summary>
    /// <param name="request">An instance of <see cref="NelcoCsvRequest"/> containing the file name of the input CSV file.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task JoinNelcoAndRsiInvoiceData(NelcoCsvRequest request)
    {
        var baseFilename = $"/Users/kc/Downloads/nelco-invoices/{request.Filename}";
        
        // File from Nelco we are importing.  Just the filename minus the extension.
        var input = $"{baseFilename}.csv";
        
        // Output file that we add info from tickets we have stored in DynamoDb.
        var output = $"{baseFilename}-rsi.csv";
        
        // Output file that contains records in Nelco csv that were not found in DynamoDb.
        var notfound = $"{baseFilename}-notfound.csv";

        var records = CsvToRecords(input);
        var notFoundRecords = await UpdateRecords(records);
        RecordsToCsv(output, records);       
        RecordsToCsv(notfound, notFoundRecords);       
    }
 
    private async Task<List<NelcoInvoice>> UpdateRecords(List<NelcoInvoice> records)
    {
        var count = 0;
        var notFoundRecords = new List<NelcoInvoice>();
        foreach (var record in records)
        {
            count++;
            var ticket = await dynamoService.GetByBatchOidAsync(record.BatchNumber);
            if (ticket != null)
            {
                record.Company = ticket.Company ?? "Company Not Found";    
                record.Cost = ticket.Cost ?? 0;
                record.Gross = ticket.Gross ?? 0;
                Console.WriteLine($"Updated: {record.Company} - record {count} of {records.Count}");
            }
            else
            {
                notFoundRecords.Add(record);
            }
        }
        return notFoundRecords;
    }
    
    private List<NelcoInvoice> CsvToRecords(string path)
    {
        using var reader = new StreamReader(path);
        
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            MissingFieldFound = null,
            HeaderValidated = null
        };        
        using var csv = new CsvReader(reader, config);
        
        csv.Context.RegisterClassMap<NelcoInvoiceMap>();
        var records = csv.GetRecords<NelcoInvoice>().ToList();        
        
        Console.WriteLine($"Loaded {records.Count} rows");
        return records;
    }

    private void RecordsToCsv(string path, List<NelcoInvoice> records)
    {
        if (records.Count == 0) return;
        
        using var writer = new StreamWriter(path);

        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true
        };

        using var csvOut = new CsvWriter(writer, config);
        csvOut.Context.RegisterClassMap<NelcoInvoiceMap>();
        csvOut.WriteHeader<NelcoInvoice>();
        csvOut.NextRecord();

        csvOut.WriteRecords(records);
    }    
}