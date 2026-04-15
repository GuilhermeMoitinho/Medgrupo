using Medgrupo.Business.Notifications.Abstractions;

namespace Medgrupo.Business.Notifications;

public class NotificationContext : INotificationContext
{
    private readonly List<Notification> _notifications = [];

    public IReadOnlyCollection<Notification> Notifications => _notifications.AsReadOnly();
    public bool HasNotifications => _notifications.Count > 0;
    public NotificationType Type { get; private set; } = NotificationType.None;

    public void AddNotification(string key, string message)
    {
        if (Type == NotificationType.None) Type = NotificationType.Validation;
        _notifications.Add(new Notification(key, message));
    }

    public void AddNotFound(string key, string message)
    {
        Type = NotificationType.NotFound;
        _notifications.Add(new Notification(key, message));
    }

    public void AddConflict(string key, string message)
    {
        Type = NotificationType.Conflict;
        _notifications.Add(new Notification(key, message));
    }
}
