using DerekNotifications.Models.Issues;

namespace DerekNotifications.Models;

public class BillMeTicketYouTrack
{
    public required string IdReadable { get; set; }
    public long Created { get; set; }
    public string? Summary { get; set; }
    public List<CustomField>? CustomFields { get; set; }
    
    public string? SerialNumber =>
        CustomFields?
            .FirstOrDefault(cf => cf.Name == "Serial Number")?
            .Value?.ToString();

    public string? Email =>
        CustomFields?
            .FirstOrDefault(cf => cf.Name == "Email")?
            .Value?.ToString();
    
    public string? Company =>
        CustomFields?
            .FirstOrDefault(cf => cf.Name == "Company")?
            .Value?.ToString();
    
    public decimal Cost =>
        Convert.ToDecimal(CustomFields?
            .FirstOrDefault(cf => cf.Name == "Cost")?
            .Value?.ToString());

    public string? BatchId =>
        CustomFields?
            .FirstOrDefault(cf => cf.Name == "Batch OID")?
            .Value?.ToString();
    
    public string? FormType =>
        CustomFields?
            .FirstOrDefault(cf => cf.Name == "FormType")?
            .Value?.ToString();

    public bool HasConfirmation
    {
        get
        {
            var value = CustomFields?.FirstOrDefault(cf => cf.Name == "HasConfirmation")?.Value;

            if (value is System.Text.Json.JsonElement element && element.ValueKind == System.Text.Json.JsonValueKind.Number)
            {
                return element.GetInt32() != 0;
            }

            return Convert.ToBoolean(value);
        }
    }    
}