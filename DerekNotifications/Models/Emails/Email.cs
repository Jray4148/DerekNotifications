using DerekNotifications.Models.Requests;

namespace DerekNotifications.Models.Emails;

public abstract class Email(Request request)
{
    protected Request Request { get; } = request;
    public string To { get; protected set; } = string.Empty;
    public string From { get; protected set; } = string.Empty;
    public string Subject { get; protected set; } = string.Empty;
    public string Body { get; protected set; } = string.Empty;
}

public abstract class Email<TRequest>(TRequest request) : Email(request) where TRequest : Request
{
    protected new TRequest Request => (TRequest)base.Request;
}