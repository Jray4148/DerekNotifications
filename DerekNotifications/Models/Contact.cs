namespace DerekNotifications.Models;

public class Contact
{
    public string? Id { get; set; }
    public string? Name { get; set; }
    
    public static Contact? Map(ContactZoho? source)
    {
        if (source == null) return null; 
        return new Contact
        {
            Id = source.Id,
            Name = source.Name
        };
    }    
}