using FluentValidation;
using Medgrupo.Business.Dtos;
using Medgrupo.Business.Notifications.Abstractions;
using Medgrupo.Data.Repositories.Abstractions;
using Medgrupo.Business.Services.Abstractions;
using Medgrupo.Data.Entities;
using Medgrupo.Data.Pagination;

namespace Medgrupo.Business.Services;

public class ContactService(
    IContactRepository repository,
    INotificationContext notification,
    IValidator<IContactInput> validator) : IContactService
{
    private readonly IContactRepository _repository = repository;
    private readonly INotificationContext _notification = notification;
    private readonly IValidator<IContactInput> _validator = validator;

    public async Task<ContactResponseDto?> CreateAsync(ContactCreateDto dto, CancellationToken ct = default)
    {
        if (!Validate(dto)) return null;

        var contact = new Contact(dto.Name.Trim(), dto.BirthDate, dto.Gender);
        await _repository.AddAsync(contact, ct);
        await _repository.SaveChangesAsync(ct);
        return ToDto(contact);
    }

    public async Task<ContactResponseDto?> UpdateAsync(Guid id, ContactUpdateDto dto, CancellationToken ct = default)
    {
        var contact = await _repository.GetActiveByIdAsync(id, ct);
        if (contact is null)
        {
            _notification.AddNotFound(nameof(id), $"Contato {id} não encontrado ou inativo.");
            return null;
        }

        if (!Validate(dto)) return null;

        contact.Update(dto.Name.Trim(), dto.BirthDate, dto.Gender);
        _repository.Update(contact);
        await _repository.SaveChangesAsync(ct);
        return ToDto(contact);
    }

    public async Task<ContactResponseDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var contact = await _repository.GetActiveByIdAsync(id, ct);
        if (contact is null)
        {
            _notification.AddNotFound(nameof(id), $"Contato {id} não encontrado ou inativo.");
            return null;
        }
        return ToDto(contact);
    }

    public async Task<PagedResult<ContactResponseDto>> GetPagedAsync(PageQuery query, CancellationToken ct = default)
    {
        var paged = await _repository.GetPagedActiveAsync(query, ct);
        var items = paged.Items.Select(ToDto).ToList();
        return new PagedResult<ContactResponseDto>(items, paged.Page, paged.PageSize, paged.TotalItems);
    }

    public async Task DeactivateAsync(Guid id, CancellationToken ct = default)
    {
        var contact = await _repository.GetByIdAsync(id, ct);
        if (contact is null)
        {
            _notification.AddNotFound(nameof(id), $"Contato {id} não encontrado.");
            return;
        }
        contact.Deactivate();
        _repository.Update(contact);
        await _repository.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var contact = await _repository.GetByIdAsync(id, ct);
        if (contact is null)
        {
            _notification.AddNotFound(nameof(id), $"Contato {id} não encontrado.");
            return;
        }
        _repository.Remove(contact);
        await _repository.SaveChangesAsync(ct);
    }

    private bool Validate(IContactInput input)
    {
        var result = _validator.Validate(input);
        if (result.IsValid) return true;

        foreach (var error in result.Errors)
            _notification.AddNotification(error.PropertyName, error.ErrorMessage);

        return false;
    }

    private static ContactResponseDto ToDto(Contact c) =>
        new(c.Id, c.Name, c.BirthDate, c.Gender, c.Age, c.Active, c.CreatedAt, c.UpdatedAt);
}
