using HRVault.Application.Absences.DTOs;
using HRVault.Application.Common.Interfaces;
using HRVault.Application.Common.Models;
using HRVault.Domain.Entities;
using HRVault.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace HRVault.Infrastructure.Persistence.Repositories;

public class EmployeeAbsenceRepository
    : IEmployeeAbsenceRepository
{
    private readonly ApplicationDbContext _context;

    public EmployeeAbsenceRepository(
        ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<EmployeeAbsence?> GetByIdAndCompanyAsync(
        Guid id,
        Guid companyId,
        CancellationToken cancellationToken = default)
    {
        return await _context.EmployeeAbsences
            .Include(x => x.Employee)
            .Include(x => x.AbsenceType)
            .FirstOrDefaultAsync(
                x =>
                    x.Id == id &&
                    x.CompanyId == companyId,
                cancellationToken);
    }

    public async Task<PagedResult<EmployeeAbsenceDto>> SearchAsync(
        EmployeeAbsenceFilterDto filter,
        Guid companyId,
        CancellationToken cancellationToken = default)
    {
        var query = _context.EmployeeAbsences
            .AsNoTracking()
            .Include(x => x.Employee)
            .Include(x => x.AbsenceType)
            .Where(x => x.CompanyId == companyId)
            .AsQueryable();

        if (filter.EmployeeId.HasValue)
        {
            query = query.Where(
                x => x.EmployeeId ==
                     filter.EmployeeId.Value);
        }
		
		if (filter.DepartmentId.HasValue)
		{
			query = query.Where(
				x => x.Employee.DepartmentId ==
					 filter.DepartmentId.Value);
		}

        if (filter.AbsenceTypeId.HasValue)
        {
            query = query.Where(
                x => x.AbsenceTypeId ==
                     filter.AbsenceTypeId.Value);
        }

        if (!string.IsNullOrWhiteSpace(
                filter.Status))
        {
            var statusText =
                filter.Status.Trim();

            if (Enum.TryParse<AbsenceStatus>(
                    statusText,
                    true,
                    out var status))
            {
                query = query.Where(
                    x => x.Status == status);
            }
            else
            {
                // Estado inválido: não devolve resultados.
                query = query.Where(x => false);
            }
        }

        // Interseção de períodos:
        // a ausência termina depois do início
        // e começa antes do fim do filtro.
        if (filter.DateFrom.HasValue)
        {
            query = query.Where(
                x => x.EndDateTime >=
                     filter.DateFrom.Value);
        }

        if (filter.DateTo.HasValue)
        {
            query = query.Where(
                x => x.StartDateTime <=
                     filter.DateTo.Value);
        }

        var totalCount =
            await query.CountAsync(
                cancellationToken);

        var items = await query
            .OrderByDescending(
                x => x.StartDateTime)
            .ThenBy(x =>
                x.Employee.LastName)
            .ThenBy(x =>
                x.Employee.FirstName)
            .Skip(
                (filter.Page - 1) *
                filter.PageSize)
            .Take(filter.PageSize)
            .Select(x =>
                new EmployeeAbsenceDto
                {
                    Id = x.Id,

                    EmployeeId =
                        x.EmployeeId,

                    EmployeeName =
                        x.Employee.FirstName +
                        " " +
                        x.Employee.LastName,

                    AbsenceTypeId =
                        x.AbsenceTypeId,

                    AbsenceTypeName =
                        x.AbsenceType.Name,

                    StartDateTime =
                        x.StartDateTime,

                    EndDateTime =
                        x.EndDateTime,

                    Status =
                        x.Status.ToString(),

                    Reason =
                        x.Reason,

                    Notes =
                        x.Notes,
						
					AbsenceTypeColor =
						x.AbsenceType.Color
                })
            .ToListAsync(
                cancellationToken);

        return new PagedResult<EmployeeAbsenceDto>
        {
            Items = items,
            TotalCount = totalCount,
            Page = filter.Page,
            PageSize = filter.PageSize
        };
    }

    public async Task<bool> HasOverlapAsync(
        Guid employeeId,
        DateTime startDateTime,
        DateTime endDateTime,
        Guid companyId,
        Guid? excludeAbsenceId = null,
        CancellationToken cancellationToken = default)
    {
        var query =
            _context.EmployeeAbsences
                .AsNoTracking()
                .Where(x =>
                    x.CompanyId == companyId &&
                    x.EmployeeId == employeeId &&
                    x.Status !=
                        AbsenceStatus.Rejected &&
                    x.Status !=
                        AbsenceStatus.Cancelled &&
                    x.StartDateTime <
                        endDateTime &&
                    x.EndDateTime >
                        startDateTime);

        if (excludeAbsenceId.HasValue)
        {
            query = query.Where(
                x =>
                    x.Id !=
                    excludeAbsenceId.Value);
        }

        return await query.AnyAsync(
            cancellationToken);
    }

    public async Task AddAsync(
        EmployeeAbsence absence,
        CancellationToken cancellationToken = default)
    {
        await _context.EmployeeAbsences
            .AddAsync(
                absence,
                cancellationToken);
    }

    public Task DeleteAsync(
        EmployeeAbsence absence,
        CancellationToken cancellationToken = default)
    {
        _context.EmployeeAbsences.Remove(
            absence);

        return Task.CompletedTask;
    }
}