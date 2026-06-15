using DerekNotifications.Models.Requests;
using DerekNotifications.Models.Emails;

namespace DerekNotifications.Interfaces;

public interface IEmailFactory
{
    public List<Email> Create(Request request);
}