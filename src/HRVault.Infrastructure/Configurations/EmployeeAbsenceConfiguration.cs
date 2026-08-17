using HRVault.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HRVault.Infrastructure.Configurations;

public class EmployeeAbsenceConfiguration
    : IEntityTypeConfiguration<EmployeeAbsence>
{
    public void Configure(
        EntityTypeBuilder<EmployeeAbsence> builder)
    {
        builder.ToTable("EmployeeAbsences");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.StartDateTime)
            .IsRequired();

        builder.Property(x => x.EndDateTime)
            .IsRequired();

        builder.Property(x => x.Status)
            .IsRequired();

        builder.Property(x => x.Reason)
            .HasMaxLength(500);

        builder.Property(x => x.Notes)
            .HasMaxLength(1000);

        builder.HasIndex(x => x.CompanyId);

        builder.HasIndex(x => x.EmployeeId);

        builder.HasIndex(x => x.AbsenceTypeId);

        builder.HasIndex(x => x.Status);

        builder.HasIndex(x => new
        {
            x.EmployeeId,
            x.StartDateTime,
            x.EndDateTime
        });

        builder.HasOne(x => x.Company)
            .WithMany()
            .HasForeignKey(x => x.CompanyId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Employee)
            .WithMany(x => x.Absences)
            .HasForeignKey(x => x.EmployeeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.AbsenceType)
            .WithMany(x => x.EmployeeAbsences)
            .HasForeignKey(x => x.AbsenceTypeId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}