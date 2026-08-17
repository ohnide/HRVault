using HRVault.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HRVault.Infrastructure.Configurations;

public class AbsenceTypeConfiguration
    : IEntityTypeConfiguration<AbsenceType>
{
    public void Configure(
        EntityTypeBuilder<AbsenceType> builder)
    {
        builder.ToTable("AbsenceTypes");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(x => x.Description)
            .HasMaxLength(500);

        builder.Property(x => x.RequiresApproval)
            .IsRequired();

        builder.Property(x => x.RequiresDocument)
            .IsRequired();

        builder.Property(x => x.IsPaid)
            .IsRequired();

        builder.HasIndex(x => new
        {
            x.CompanyId,
            x.Name
        })
        .IsUnique()
        .HasFilter("\"IsDeleted\" = false");

        builder.HasOne(x => x.Company)
            .WithMany()
            .HasForeignKey(x => x.CompanyId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}