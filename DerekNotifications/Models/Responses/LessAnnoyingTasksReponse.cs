using System.Text.Json.Serialization;

namespace DerekNotifications.Models.Responses;

public class LessAnnoyingTasksResponse
{
    public bool HasMoreResults { get; set; }

    public List<LessAnnoyingTaskResult> Results { get; set; } = [];
}

public class LessAnnoyingTaskResult
{
    public string TaskId { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public DateOnly DueDate { get; set; }

    public string AssignedTo { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string ContactId { get; set; } = string.Empty;

    public bool IsCompleted { get; set; }

    public DateTimeOffset? DateCompleted { get; set; }

    public string CalendarId { get; set; } = string.Empty;

    public DateTimeOffset DateCreated { get; set; }

    public LessAnnoyingAssignedToMetaData AssignedToMetaData { get; set; } = new();

    public LessAnnoyingContactMetaData ContactMetaData { get; set; } = new();
}

public class LessAnnoyingAssignedToMetaData
{
    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;
}

public class LessAnnoyingContactMetaData
{
    public string Name { get; set; } = string.Empty;

    public string AssignedTo { get; set; } = string.Empty;
}