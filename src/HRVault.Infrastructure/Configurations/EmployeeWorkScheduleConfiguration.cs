using HRVault.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HRVault.Infrastructure.Configurations;

public class EmployeeWorkScheduleConfiguration
    : IEntityTypeConfiguration<EmployeeWorkSchedule>
{
    public void Configure(EntityTypeBuilder<EmployeeWorkSchedule> builder)
    {
        builder.ToTable("EmployeeWorkSchedules");

        builder.HasKey(x => x.Id);

        builder.HasIndex(x => new
        {
            x.CompanyId,
            x.EmployeeId,
            x.StartDate
        });

        builder.HasOne(x => x.Employee)
            .WithMany(x => x.WorkSchedules)
            .HasForeignKey(x => x.EmployeeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.WorkSchedule)
            .WithMany(x => x.EmployeeAssignments)
            .HasForeignKey(x => x.WorkScheduleId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
