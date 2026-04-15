using Medgrupo.Business.Notifications;

namespace Medgrupo.Business.Notifications.Abstractions;

public interface INotificationContext
{
    IReadOnlyCollection<Notification> Notifications { get; }
    bool HasNotifications { get; }
    NotificationType Type { get; }
    void AddNotification(string key, string message);
    void AddNotFound(string key, string message);
    void AddConflict(string key, string message);
}

public enum NotificationType
{
    None = 0,
    Validation = 400,
    NotFound = 404,
    Conflict = 409
}
