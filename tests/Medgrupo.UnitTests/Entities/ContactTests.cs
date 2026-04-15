using FluentAssertions;
using Medgrupo.Data.Entities;
using Xunit;

namespace Medgrupo.UnitTests.Entities;

public class ContactTests
{
    [Fact]
    public void Constructor_Should_Initialize_Active_And_Id()
    {
        var c = new Contact("Ana", DateTime.Today.AddYears(-25), Gender.Female);
        c.Active.Should().BeTrue();
        c.Id.Should().NotBe(Guid.Empty);
        c.Age.Should().Be(25);
    }

    [Fact]
    public void Deactivate_Should_Set_Active_False_And_Update_Timestamp()
    {
        var c = new Contact("Ana", DateTime.Today.AddYears(-25), Gender.Female);
        c.Deactivate();
        c.Active.Should().BeFalse();
        c.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public void Activate_Should_Set_Active_True()
    {
        var c = new Contact("Ana", DateTime.Today.AddYears(-25), Gender.Female);
        c.Deactivate();
        c.Activate();
        c.Active.Should().BeTrue();
    }

    [Fact]
    public void Update_Should_Change_Fields()
    {
        var c = new Contact("Ana", DateTime.Today.AddYears(-25), Gender.Female);
        c.Update("Ana Maria", DateTime.Today.AddYears(-30), Gender.Other);
        c.Name.Should().Be("Ana Maria");
        c.Age.Should().Be(30);
        c.Gender.Should().Be(Gender.Other);
    }

    [Theory]
    [InlineData(-18, 18)]
    [InlineData(-30, 30)]
    [InlineData(-1, 1)]
    public void Age_Should_Be_Calculated_From_BirthDate(int yearsOffset, int expected)
    {
        var c = new Contact("x", DateTime.Today.AddYears(yearsOffset), Gender.Male);
        c.Age.Should().Be(expected);
    }
}
