namespace HRVault.Application.Common.Interfaces;

public interface ICompanyTimeZoneService
{
    Task<TimeZoneInfo> GetAsync(
        Guid companyId,
        CancellationToken cancellationToken = default);

    DateTime ConvertUtcToLocal(
        DateTime utc,
        TimeZoneInfo timeZone);

    DateTime ConvertLocalToUtc(
        DateTime local,
        TimeZoneInfo timeZone);

    (DateTime FromUtc, DateTime ToUtc) GetUtcDayRange(
        DateOnly localDate,
        TimeZoneInfo timeZone);
}
