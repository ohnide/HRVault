using HRVault.Application.Common.Interfaces;
using HRVault.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HRVault.Infrastructure.Persistence.Repositories;

public class DocumentRepository
    : IDocumentRepository
{
    private readonly ApplicationDbContext _context;

    public DocumentRepository(
        ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Document?> GetByIdAndEmployeeIdAsync(
        Guid id,
        Guid employeeId,
        CancellationToken cancellationToken = default)
    {
        return await _context.Documents
            .FirstOrDefaultAsync(
                x => x.Id == id &&
                     x.EmployeeId == employeeId,
                cancellationToken);
    }

    public async Task<List<Document>> GetAllByEmployeeIdAsync(
        Guid employeeId,
        CancellationToken cancellationToken = default)
    {
        return await _context.Documents
            .AsNoTracking()
            .Where(x => x.EmployeeId == employeeId)
            .OrderByDescending(x => x.UploadedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(
        Document document,
        CancellationToken cancellationToken = default)
    {
        await _context.Documents.AddAsync(
            document,
            cancellationToken);
    }

    public Task DeleteAsync(
        Document document,
        CancellationToken cancellationToken = default)
    {
        _context.Documents.Remove(document);

        return Task.CompletedTask;
    }
}