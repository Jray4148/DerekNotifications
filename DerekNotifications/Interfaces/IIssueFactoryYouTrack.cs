using DerekNotifications.Models.Issues;
using DerekNotifications.Models.Requests;

namespace DerekNotifications.Interfaces;

public interface IIssueFactoryYouTrack
{
    Issue Create(Request request);
}