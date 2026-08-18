using HRVault.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HRVault.Infrastructure.Persistence.Configurations;

public class VacationBalanceConfiguration
    : IEntityTypeConfiguration<VacationBalance>
{
    public void Configure(
        EntityTypeBuilder<VacationBalance> builder)
    {
        builder.ToTable("VacationBalances");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Year)
            .IsRequired();

        builder.Property(x => x.EntitledDays)
            .HasPrecision(5, 2)
            .IsRequired();

        builder.Property(x => x.CarriedOverDays)
            .HasPrecision(5, 2)
            .IsRequired();

        builder.Property(x => x.AdjustmentDays)
            .HasPrecision(5, 2)
            .IsRequired();

        builder.Property(x => x.Notes)
            .HasMaxLength(1000);

        builder.HasOne(x => x.Company)
            .WithMany()
            .HasForeignKey(x => x.CompanyId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Employee)
            .WithMany()
            .HasForeignKey(x => x.EmployeeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => new
        {
            x.CompanyId,
            x.EmployeeId,
            x.Year
        })
        .IsUnique();
    }
}