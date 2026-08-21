using HRVault.Application.Common.Interfaces;
using HRVault.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HRVault.Infrastructure.Services;

public class CompanyTimeZoneService
    : ICompanyTimeZoneService
{
    private readonly ApplicationDbContext _context;

    public CompanyTimeZoneService(
        ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<TimeZoneInfo> GetAsync(
        Guid companyId,
        CancellationToken cancellationToken = default)
    {
        var timeZoneId =
            await _context.Companies
                .AsNoTracking()
                .Where(x => x.Id == companyId)
                .Select(x => x.TimeZoneId)
                .FirstOrDefaultAsync(cancellationToken);

        if (string.IsNullOrWhiteSpace(timeZoneId))
            timeZoneId = "Europe/Lisbon";

        try
        {
            return TimeZoneInfo.FindSystemTimeZoneById(
                timeZoneId);
        }
        catch (TimeZoneNotFoundException)
        {
            return TimeZoneInfo.FindSystemTimeZoneById(
                "Europe/Lisbon");
        }
        catch (InvalidTimeZoneException)
        {
            return TimeZoneInfo.FindSystemTimeZoneById(
                "Europe/Lisbon");
        }
    }

    public DateTime ConvertUtcToLocal(
        DateTime utc,
        TimeZoneInfo timeZone)
    {
        var normalizedUtc =
            utc.Kind == DateTimeKind.Utc
                ? utc
                : DateTime.SpecifyKind(
                    utc,
                    DateTimeKind.Utc);

        return TimeZoneInfo.ConvertTimeFromUtc(
            normalizedUtc,
            timeZone);
    }

    public DateTime ConvertLocalToUtc(
        DateTime local,
        TimeZoneInfo timeZone)
    {
        var unspecified =
            DateTime.SpecifyKind(
                local,
                DateTimeKind.Unspecified);

        return TimeZoneInfo.ConvertTimeToUtc(
            unspecified,
            timeZone);
    }

    public (DateTime FromUtc, DateTime ToUtc) GetUtcDayRange(
        DateOnly localDate,
        TimeZoneInfo timeZone)
    {
        var localStart =
            localDate.ToDateTime(
                TimeOnly.MinValue,
                DateTimeKind.Unspecified);

        var localEnd =
            localDate.AddDays(1)
                .ToDateTime(
                    TimeOnly.MinValue,
                    DateTimeKind.Unspecified);

        return (
            ConvertLocalToUtc(localStart, timeZone),
            ConvertLocalToUtc(localEnd, timeZone));
    }
}
