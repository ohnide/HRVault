using HRVault.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HRVault.Infrastructure.Configurations;

public class EmployeeConfiguration
    : IEntityTypeConfiguration<Employee>
{
    public void Configure(
        EntityTypeBuilder<Employee> builder)
    {
        builder.ToTable("Employees");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.EmployeeNumber)
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(x => x.FirstName)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.LastName)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.WorkEmail)
            .HasMaxLength(200);

        builder.Property(x => x.PersonalEmail)
            .HasMaxLength(200);

        builder.Property(x => x.MobilePhone)
            .HasMaxLength(30);

        builder.HasIndex(x => new
        {
            x.CompanyId,
            x.EmployeeNumber
        })
        .IsUnique()
        .HasFilter("\"IsDeleted\" = false");

        builder.HasOne(x => x.Company)
            .WithMany(x => x.Employees)
            .HasForeignKey(x => x.CompanyId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Department)
            .WithMany(x => x.Employees)
            .HasForeignKey(x => x.DepartmentId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(x => x.Position)
            .WithMany(x => x.Employees)
            .HasForeignKey(x => x.PositionId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(x => x.Profile)
            .WithOne(x => x.Employee)
            .HasForeignKey<EmployeeProfile>(
                x => x.EmployeeId);

        builder.HasOne(x => x.EmergencyContact)
            .WithOne(x => x.Employee)
            .HasForeignKey<EmployeeEmergencyContact>(
                x => x.EmployeeId);

        builder.HasMany(x => x.Addresses)
            .WithOne(x => x.Employee)
            .HasForeignKey(x => x.EmployeeId);

        builder.HasMany(x => x.Contacts)
            .WithOne(x => x.Employee)
            .HasForeignKey(x => x.EmployeeId);

        builder.HasMany(x => x.Documents)
            .WithOne(x => x.Employee)
            .HasForeignKey(x => x.EmployeeId);
    }
}