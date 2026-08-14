using HRVault.Domain.Entities;
using HRVault.Infrastructure.Persistence.Configurations.Base;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace HRVault.Infrastructure.Configurations;

public class CompanyConfiguration
    : BaseEntityConfiguration<Company>
{
    protected override void ConfigureEntity(
        EntityTypeBuilder<Company> builder)
    {
        builder.ToTable("Companies");

        builder.Property(x => x.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.VatNumber)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.LogoUrl)
            .HasMaxLength(500);

        builder.Property(x => x.Address)
            .HasMaxLength(500);

        builder.HasIndex(x => x.VatNumber)
            .IsUnique();

        builder.HasMany(x => x.Departments)
            .WithOne(x => x.Company)
            .HasForeignKey(x => x.CompanyId);

        builder.HasMany(x => x.Users)
            .WithOne(x => x.Company)
            .HasForeignKey(x => x.CompanyId);

        builder.HasMany(x => x.Employees)
            .WithOne(x => x.Company)
            .HasForeignKey(x => x.CompanyId);
    }
}