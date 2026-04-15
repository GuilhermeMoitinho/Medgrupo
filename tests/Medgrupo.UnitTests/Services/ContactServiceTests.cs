using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Medgrupo.Business.Dtos;
using Medgrupo.Business.Notifications.Abstractions;
using Medgrupo.Business.Services;
using Medgrupo.Data.Entities;
using Medgrupo.Data.Pagination;
using Medgrupo.Data.Repositories.Abstractions;
using NSubstitute;
using Xunit;

namespace Medgrupo.UnitTests.Services;

public class ContactServiceTests
{
    private readonly IContactRepository _repository = Substitute.For<IContactRepository>();
    private readonly INotificationContext _notification = Substitute.For<INotificationContext>();
    private readonly IValidator<IContactInput> _validator = Substitute.For<IValidator<IContactInput>>();
    private readonly ContactService _sut;

    public ContactServiceTests()
    {
        _sut = new ContactService(_repository, _notification, _validator);
        _validator.Validate(Arg.Any<IContactInput>()).Returns(new ValidationResult());
    }

    private static ContactCreateDto ValidCreate(string name = "Ana") =>
        new(name, DateTime.Today.AddYears(-30), Gender.Female);

    [Fact]
    public async Task CreateAsync_Should_Persist_When_Valid()
    {
        var dto = ValidCreate();

        var result = await _sut.CreateAsync(dto);

        result.Should().NotBeNull();
        result!.Name.Should().Be("Ana");
        result.Age.Should().Be(30);
        result.Active.Should().BeTrue();
        await _repository.Received(1).AddAsync(Arg.Any<Contact>(), Arg.Any<CancellationToken>());
        await _repository.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CreateAsync_Should_Return_Null_And_Notify_When_Invalid()
    {
        _validator.Validate(Arg.Any<IContactInput>())
            .Returns(new ValidationResult(new[] { new ValidationFailure("Name", "obrigatório") }));

        var result = await _sut.CreateAsync(ValidCreate(""));

        result.Should().BeNull();
        _notification.Received(1).AddNotification("Name", "obrigatório");
        await _repository.DidNotReceive().AddAsync(Arg.Any<Contact>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task UpdateAsync_Should_Return_Null_And_Add_NotFound_When_Contact_Missing()
    {
        _repository.GetActiveByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns((Contact?)null);

        var dto = new ContactUpdateDto("Ana", DateTime.Today.AddYears(-20), Gender.Female);
        var result = await _sut.UpdateAsync(Guid.NewGuid(), dto);

        result.Should().BeNull();
        _notification.Received(1).AddNotFound(Arg.Any<string>(), Arg.Any<string>());
    }

    [Fact]
    public async Task UpdateAsync_Should_Persist_When_Valid()
    {
        var existing = new Contact("Ana", DateTime.Today.AddYears(-30), Gender.Female);
        _repository.GetActiveByIdAsync(existing.Id, Arg.Any<CancellationToken>()).Returns(existing);

        var dto = new ContactUpdateDto("Ana Maria", DateTime.Today.AddYears(-25), Gender.Female);
        var result = await _sut.UpdateAsync(existing.Id, dto);

        result.Should().NotBeNull();
        result!.Name.Should().Be("Ana Maria");
        result.Age.Should().Be(25);
        _repository.Received(1).Update(existing);
        await _repository.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetByIdAsync_Should_Return_Null_And_NotFound_When_Missing()
    {
        _repository.GetActiveByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns((Contact?)null);

        var result = await _sut.GetByIdAsync(Guid.NewGuid());

        result.Should().BeNull();
        _notification.Received(1).AddNotFound(Arg.Any<string>(), Arg.Any<string>());
    }

    [Fact]
    public async Task GetByIdAsync_Should_Return_Dto_When_Found()
    {
        var contact = new Contact("Bruno", DateTime.Today.AddYears(-40), Gender.Male);
        _repository.GetActiveByIdAsync(contact.Id, Arg.Any<CancellationToken>()).Returns(contact);

        var result = await _sut.GetByIdAsync(contact.Id);

        result.Should().NotBeNull();
        result!.Name.Should().Be("Bruno");
        result.Age.Should().Be(40);
    }

    [Fact]
    public async Task GetPagedAsync_Should_Map_Items_To_Dto()
    {
        var list = new List<Contact>
        {
            new("A", DateTime.Today.AddYears(-20), Gender.Female),
            new("B", DateTime.Today.AddYears(-22), Gender.Male)
        };
        _repository.GetPagedActiveAsync(Arg.Any<PageQuery>(), Arg.Any<CancellationToken>())
            .Returns(new PagedResult<Contact>(list, 1, 10, 2));

        var result = await _sut.GetPagedAsync(new PageQuery(1, 10));

        result.Items.Should().HaveCount(2);
        result.TotalItems.Should().Be(2);
        result.Items.Select(i => i.Name).Should().Equal("A", "B");
    }

    [Fact]
    public async Task DeactivateAsync_Should_Deactivate_And_Save()
    {
        var contact = new Contact("Carla", DateTime.Today.AddYears(-30), Gender.Female);
        _repository.GetByIdAsync(contact.Id, Arg.Any<CancellationToken>()).Returns(contact);

        await _sut.DeactivateAsync(contact.Id);

        contact.Active.Should().BeFalse();
        _repository.Received(1).Update(contact);
        await _repository.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task DeactivateAsync_Should_Add_NotFound_When_Missing()
    {
        _repository.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns((Contact?)null);

        await _sut.DeactivateAsync(Guid.NewGuid());

        _notification.Received(1).AddNotFound(Arg.Any<string>(), Arg.Any<string>());
        _repository.DidNotReceive().Update(Arg.Any<Contact>());
    }

    [Fact]
    public async Task DeleteAsync_Should_Remove_And_Save()
    {
        var contact = new Contact("Diego", DateTime.Today.AddYears(-30), Gender.Male);
        _repository.GetByIdAsync(contact.Id, Arg.Any<CancellationToken>()).Returns(contact);

        await _sut.DeleteAsync(contact.Id);

        _repository.Received(1).Remove(contact);
        await _repository.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task DeleteAsync_Should_Add_NotFound_When_Missing()
    {
        _repository.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns((Contact?)null);

        await _sut.DeleteAsync(Guid.NewGuid());

        _notification.Received(1).AddNotFound(Arg.Any<string>(), Arg.Any<string>());
        _repository.DidNotReceive().Remove(Arg.Any<Contact>());
    }
}
