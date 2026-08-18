using HRVault.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HRVault.Infrastructure.Persistence.Configurations;

public class VacationRequestConfiguration
    : IEntityTypeConfiguration<VacationRequest>
{
    public void Configure(
        EntityTypeBuilder<VacationRequest> builder)
    {
        builder.ToTable("VacationRequests");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.StartDate)
            .IsRequired();

        builder.Property(x => x.EndDate)
            .IsRequired();

        builder.Property(x => x.Days)
            .HasPrecision(5, 2)
            .IsRequired();

        builder.Property(x => x.Status)
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

        builder.HasIndex(x => x.CompanyId);

        builder.HasIndex(x => x.EmployeeId);

        builder.HasIndex(x => x.Status);

        builder.HasIndex(x => new
        {
            x.EmployeeId,
            x.StartDate,
            x.EndDate
        });
    }
}