using FluentAssertions;
using Medgrupo.Business.Notifications;
using Medgrupo.Business.Notifications.Abstractions;
using Xunit;

namespace Medgrupo.UnitTests.Notifications;

public class NotificationContextTests
{
    [Fact]
    public void Should_Start_Empty_And_None()
    {
        var ctx = new NotificationContext();
        ctx.HasNotifications.Should().BeFalse();
        ctx.Type.Should().Be(NotificationType.None);
    }

    [Fact]
    public void AddNotification_Should_Set_Validation_Type()
    {
        var ctx = new NotificationContext();
        ctx.AddNotification("name", "obrigatório");

        ctx.HasNotifications.Should().BeTrue();
        ctx.Type.Should().Be(NotificationType.Validation);
        ctx.Notifications.Should().ContainSingle(n => n.Key == "name" && n.Message == "obrigatório");
    }

    [Fact]
    public void AddNotFound_Should_Set_NotFound_Type()
    {
        var ctx = new NotificationContext();
        ctx.AddNotFound("id", "missing");
        ctx.Type.Should().Be(NotificationType.NotFound);
    }

    [Fact]
    public void AddConflict_Should_Set_Conflict_Type()
    {
        var ctx = new NotificationContext();
        ctx.AddConflict("id", "dup");
        ctx.Type.Should().Be(NotificationType.Conflict);
    }
}
