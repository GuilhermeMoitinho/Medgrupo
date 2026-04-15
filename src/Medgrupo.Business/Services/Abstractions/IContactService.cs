using Medgrupo.Business.Dtos;
using Medgrupo.Data.Pagination;

namespace Medgrupo.Business.Services.Abstractions;

public interface IContactService
{
    Task<ContactResponseDto?> CreateAsync(ContactCreateDto dto, CancellationToken ct = default);
    Task<ContactResponseDto?> UpdateAsync(Guid id, ContactUpdateDto dto, CancellationToken ct = default);
    Task<ContactResponseDto?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<PagedResult<ContactResponseDto>> GetPagedAsync(PageQuery query, CancellationToken ct = default);
    Task DeactivateAsync(Guid id, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
}
