using HRVault.Application.Common.Interfaces;
using HRVault.Domain.Entities;
using HRVault.SharedKernel.Common;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace HRVault.Infrastructure.Persistence;

public class ApplicationDbContext : DbContext
{
    private readonly ICurrentUserService _currentUser;

    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options,
        ICurrentUserService currentUser)
        : base(options)
    {
        _currentUser = currentUser;
    }

    public DbSet<Company> Companies => Set<Company>();

    public DbSet<Department> Departments => Set<Department>();

    public DbSet<Position> Positions => Set<Position>();

    public DbSet<User> Users => Set<User>();

    public DbSet<Employee> Employees => Set<Employee>();

    public DbSet<Document> Documents => Set<Document>();

    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    public DbSet<Role> Roles => Set<Role>();

    public DbSet<UserRole> UserRoles => Set<UserRole>();

    public DbSet<Permission> Permissions => Set<Permission>();

    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();

    public DbSet<EmployeeProfile> EmployeeProfiles => Set<EmployeeProfile>();

    public DbSet<EmployeeAddress> EmployeeAddresses => Set<EmployeeAddress>();

    public DbSet<EmployeeContact> EmployeeContacts => Set<EmployeeContact>();

    public DbSet<EmployeeEmergencyContact> EmployeeEmergencyContacts =>
        Set<EmployeeEmergencyContact>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(ApplicationDbContext).Assembly);

        ApplySoftDeleteQueryFilters(modelBuilder);
    }

    public override int SaveChanges()
    {
        PrepareEntitiesForSave();

        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        PrepareEntitiesForSave();

        return base.SaveChangesAsync(cancellationToken);
    }

    private void PrepareEntitiesForSave()
    {
        UpdateSoftDeleteEntities();
        UpdateAuditableEntities();
    }

    private void UpdateAuditableEntities()
    {
        var entries = ChangeTracker
            .Entries<AuditableEntity>();

        var userId = _currentUser.UserId;

        foreach (var entry in entries)
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAt = DateTime.UtcNow;

                    if (userId.HasValue)
                        entry.Entity.CreatedBy = userId.Value;

                    break;

                case EntityState.Modified:
                    entry.Entity.UpdatedAt = DateTime.UtcNow;

                    if (userId.HasValue)
                        entry.Entity.UpdatedBy = userId.Value;

                    break;
            }
        }
    }

    private void UpdateSoftDeleteEntities()
    {
        var entries = ChangeTracker
            .Entries<SoftDeleteEntity>();

        var userId = _currentUser.UserId;

        foreach (var entry in entries)
        {
            if (entry.State != EntityState.Deleted)
                continue;

            entry.State = EntityState.Modified;

            entry.Entity.IsDeleted = true;
            entry.Entity.DeletedAt = DateTime.UtcNow;

            if (userId.HasValue)
                entry.Entity.DeletedBy = userId.Value;
        }
    }

    private static void ApplySoftDeleteQueryFilters(
        ModelBuilder modelBuilder)
    {
        var softDeleteEntities = modelBuilder.Model
            .GetEntityTypes()
            .Where(x =>
                typeof(SoftDeleteEntity)
                    .IsAssignableFrom(x.ClrType));

        foreach (var entityType in softDeleteEntities)
        {
            var parameter = Expression.Parameter(
                entityType.ClrType,
                "e");

            var property = Expression.Property(
                parameter,
                nameof(SoftDeleteEntity.IsDeleted));

            var compare = Expression.Equal(
                property,
                Expression.Constant(false));

            var lambda = Expression.Lambda(
                compare,
                parameter);

            modelBuilder.Entity(entityType.ClrType)
                .HasQueryFilter(lambda);
        }
    }
}