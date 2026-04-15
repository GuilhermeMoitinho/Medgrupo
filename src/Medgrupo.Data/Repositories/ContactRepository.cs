using Medgrupo.Data.Context;
using Medgrupo.Data.Entities;
using Medgrupo.Data.Pagination;
using Medgrupo.Data.Repositories.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Medgrupo.Data.Repositories;

public class ContactRepository(AppDbContext context) : IContactRepository
{
    private readonly AppDbContext _context = context;

    public Task<Contact?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        _context.Contacts.FirstOrDefaultAsync(c => c.Id == id, ct);

    public Task<Contact?> GetActiveByIdAsync(Guid id, CancellationToken ct = default) =>
        _context.Contacts.FirstOrDefaultAsync(c => c.Id == id && c.Active, ct);

    public async Task<PagedResult<Contact>> GetPagedActiveAsync(PageQuery query, CancellationToken ct = default)
    {
        var baseQuery = _context.Contacts.AsNoTracking().Where(c => c.Active);
        var total = await baseQuery.CountAsync(ct);
        var items = await baseQuery
            .OrderBy(c => c.Name)
            .Skip(query.Skip)
            .Take(query.NormalizedPageSize)
            .ToListAsync(ct);
        return new PagedResult<Contact>(items, query.NormalizedPage, query.NormalizedPageSize, total);
    }

    public async Task AddAsync(Contact contact, CancellationToken ct = default) =>
        await _context.Contacts.AddAsync(contact, ct);

    public void Update(Contact contact) => _context.Contacts.Update(contact);

    public void Remove(Contact contact) => _context.Contacts.Remove(contact);

    public async Task<bool> SaveChangesAsync(CancellationToken ct = default) =>
        await _context.SaveChangesAsync(ct) > 0;
}
