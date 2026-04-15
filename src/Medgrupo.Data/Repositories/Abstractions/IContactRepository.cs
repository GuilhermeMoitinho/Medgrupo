using Medgrupo.Data.Entities;
using Medgrupo.Data.Pagination;

namespace Medgrupo.Data.Repositories.Abstractions;

public interface IContactRepository
{
    Task<Contact?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<Contact?> GetActiveByIdAsync(Guid id, CancellationToken ct = default);
    Task<PagedResult<Contact>> GetPagedActiveAsync(PageQuery query, CancellationToken ct = default);
    Task AddAsync(Contact contact, CancellationToken ct = default);
    void Update(Contact contact);
    void Remove(Contact contact);
    Task<bool> SaveChangesAsync(CancellationToken ct = default);
}
