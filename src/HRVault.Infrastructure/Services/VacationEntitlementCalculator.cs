using HRVault.Application.Common.Interfaces;

namespace HRVault.Infrastructure.Services;

public class VacationEntitlementCalculator
    : IVacationEntitlementCalculator
{
    public decimal Calculate(
        DateOnly hireDate,
        int year)
    {
        if (year < hireDate.Year)
        {
            return 0;
        }

        if (year > hireDate.Year)
        {
            return 22;
        }

        var months =
            12 - hireDate.Month + 1;

        var days =
            months * 2;

        return Math.Min(days, 20);
    }
}