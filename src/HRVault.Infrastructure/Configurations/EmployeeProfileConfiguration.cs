using HRVault.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HRVault.Infrastructure.Configurations;

public class EmployeeProfileConfiguration : IEntityTypeConfiguration<EmployeeProfile>
{
    public void Configure(EntityTypeBuilder<EmployeeProfile> builder)
    {
        builder.ToTable("EmployeeProfiles");

        builder.HasKey(x => x.Id);

        builder.HasOne(x => x.Employee)
            .WithOne(x => x.Profile)
            .HasForeignKey<EmployeeProfile>(x => x.EmployeeId);

        builder.Property(x => x.Nationality)
            .HasMaxLength(100);

        builder.Property(x => x.DocumentNumber)
            .HasMaxLength(50);

        builder.Property(x => x.TaxNumber)
            .HasMaxLength(30);

        builder.Property(x => x.SocialSecurityNumber)
            .HasMaxLength(30);

        builder.Property(x => x.SnsNumber)
            .HasMaxLength(30);
    }
}