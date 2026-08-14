using HRVault.Application.Common.Interfaces;

namespace HRVault.Infrastructure.Services;

public class DateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
}