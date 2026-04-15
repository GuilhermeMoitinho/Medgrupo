using FluentAssertions;
using Medgrupo.Business.Dtos;
using Medgrupo.Business.Validators;
using Medgrupo.Data.Entities;
using Xunit;

namespace Medgrupo.UnitTests.Validators;

public class ContactInputValidatorTests
{
    private readonly ContactInputValidator _sut = new();

    [Fact]
    public void Should_Pass_When_Valid()
    {
        var dto = new ContactCreateDto("João", DateTime.Today.AddYears(-25), Gender.Male);
        var result = _sut.Validate(dto);
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Should_Fail_When_Name_Is_Empty()
    {
        var dto = new ContactCreateDto("", DateTime.Today.AddYears(-25), Gender.Male);
        var result = _sut.Validate(dto);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(IContactInput.Name));
    }

    [Fact]
    public void Should_Fail_When_BirthDate_In_Future()
    {
        var dto = new ContactCreateDto("João", DateTime.Today.AddDays(1), Gender.Male);
        var result = _sut.Validate(dto);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage.Contains("maior que hoje"));
    }

    [Fact]
    public void Should_Fail_When_Age_Is_Zero()
    {
        var dto = new ContactCreateDto("João", DateTime.Today, Gender.Male);
        var result = _sut.Validate(dto);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage.Contains("igual a 0"));
    }

    [Fact]
    public void Should_Fail_When_Minor()
    {
        var dto = new ContactCreateDto("João", DateTime.Today.AddYears(-10), Gender.Male);
        var result = _sut.Validate(dto);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage.Contains("maior de idade"));
    }
}
