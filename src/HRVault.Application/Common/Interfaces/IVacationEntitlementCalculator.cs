namespace HRVault.Application.Common.Interfaces;

public interface IVacationEntitlementCalculator
{
    decimal Calculate(
        DateOnly hireDate,
        int year);
}