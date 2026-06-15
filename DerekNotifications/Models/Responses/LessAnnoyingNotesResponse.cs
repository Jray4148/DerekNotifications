namespace DerekNotifications.Models.Responses;

public class LessAnnoyingNotesResponse
{
    public bool HasMoreResults { get; set; }

    public List<LessAnnoyingNoteResult> Results { get; set; } = [];
}

public class LessAnnoyingNoteResult
{
    public string? ContactId { get; set; }

    public ContactMetaData? ContactMetaData { get; set; }

    public DateTimeOffset DateCreated { get; set; }

    public DateTimeOffset DateDisplayedInHistory { get; set; }

    public bool IsRichText { get; set; }

    public string? Note { get; set; }

    public string? NoteId { get; set; }

    public PipelineInfo? PipelineInfo { get; set; }

    public string? UserId { get; set; }

    public UserMetaData? UserMetaData { get; set; }
}

public class ContactMetaData
{
    public string? Name { get; set; }

    public string? AssignedTo { get; set; }
}

public class UserMetaData
{
    public string? FirstName { get; set; }

    public string? LastName { get; set; }
}

public class PipelineInfo
{
    public string? PipelineId { get; set; }

    public string? PipelineItemId { get; set; }

    public string? StatusId { get; set; }

    public string? PreviousStatusId { get; set; }

    public PipelineMetaData? PipelineMetaData { get; set; }
}

public class PipelineMetaData
{
}