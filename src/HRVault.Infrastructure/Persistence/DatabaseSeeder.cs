using HRVault.Application.Common.Interfaces;
using HRVault.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HRVault.Infrastructure.Persistence;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(
        ApplicationDbContext context,
        IPasswordHasher passwordHasher)
    {
        await context.Database.MigrateAsync();

        // -------------------------------------------------
        // 1. Criar empresa inicial
        // -------------------------------------------------

        var company = await context.Companies
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync();

        if (company is null)
        {
            company = new Company
            {
                Name = "HRVault",
                VatNumber = "999999990",
                Address = null,
                LogoUrl = null
            };

            context.Companies.Add(company);

            await context.SaveChangesAsync();
        }

        // -------------------------------------------------
        // 2. Criar utilizador administrador da plataforma
        // -------------------------------------------------

        const string adminEmail = "admin@hrvault.pt";

        var adminUser = await context.Users
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(
                x => x.Email == adminEmail);

        if (adminUser is null)
        {
            adminUser = new User
            {
                CompanyId = company.Id,
                Name = "Administrador",
                Email = adminEmail,
                PasswordHash =
                    passwordHasher.Hash("Admin123!"),
                IsAdministrator = true,
                IsPlatformAdministrator = true,
                IsActive = true
            };

            context.Users.Add(adminUser);

            await context.SaveChangesAsync();
        }
        else
        {
            var changed = false;

            if (!adminUser.IsAdministrator)
            {
                adminUser.IsAdministrator = true;
                changed = true;
            }

            if (!adminUser.IsPlatformAdministrator)
            {
                adminUser.IsPlatformAdministrator = true;
                changed = true;
            }

            if (!adminUser.IsActive)
            {
                adminUser.IsActive = true;
                changed = true;
            }

            if (changed)
            {
                context.Users.Update(adminUser);

                await context.SaveChangesAsync();
            }
        }

        // -------------------------------------------------
        // 3. Criar permissões globais
        // -------------------------------------------------

        var permissionDefinitions = new[]
        {
            ("Employees.View", "Ver funcionários"),
            ("Employees.Create", "Criar funcionários"),
            ("Employees.Update", "Editar funcionários"),
            ("Employees.Delete", "Eliminar funcionários"),

			("Documents.View", "Ver documentos"),
			("Documents.Upload", "Carregar documentos"),
			("Documents.Delete", "Eliminar documentos"),
			("Documents.ManageTypes", "Gerir tipos de documentos"),

			("Departments.View", "Ver departamentos"),
            ("Departments.Create", "Criar departamentos"),
            ("Departments.Update", "Editar departamentos"),
            ("Departments.Delete", "Eliminar departamentos"),

            ("Positions.View", "Ver cargos"),
            ("Positions.Create", "Criar cargos"),
            ("Positions.Update", "Editar cargos"),
            ("Positions.Delete", "Eliminar cargos"),

            ("Users.View", "Ver utilizadores"),
            ("Users.Create", "Criar utilizadores"),
            ("Users.Update", "Editar utilizadores"),
            ("Users.Delete", "Eliminar utilizadores"),
            ("Users.ResetPassword", "Reset Password"),

            ("Roles.View", "Ver funções"),
            ("Roles.Create", "Criar funções"),
            ("Roles.Update", "Editar funções"),
            ("Roles.Delete", "Eliminar funções")
        };

        foreach (var definition in permissionDefinitions)
        {
            var exists = await context.Permissions
                .IgnoreQueryFilters()
                .AnyAsync(
                    x => x.Code == definition.Item1);

            if (!exists)
            {
                context.Permissions.Add(
                    new Permission
                    {
                        Code = definition.Item1,
                        Name = definition.Item2,
                        Description = definition.Item2,
                        IsActive = true
                    });
            }
        }

        await context.SaveChangesAsync();

        // -------------------------------------------------
        // 4. Criar Role Administrador da empresa
        // -------------------------------------------------

        var adminRole = await context.Roles
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(
                x =>
                    x.CompanyId == company.Id &&
                    x.Name == "Administrador");

        if (adminRole is null)
        {
            adminRole = new Role
            {
                CompanyId = company.Id,
                Name = "Administrador",
                Description = "Acesso total ao sistema"
            };

            context.Roles.Add(adminRole);

            await context.SaveChangesAsync();
        }

        // -------------------------------------------------
        // 5. Atribuir todas as permissões ao Administrador
        // -------------------------------------------------

        var permissions = await context.Permissions
            .IgnoreQueryFilters()
            .Where(x => x.IsActive)
            .ToListAsync();

        foreach (var permission in permissions)
        {
            var exists = await context.RolePermissions
                .AnyAsync(
                    x =>
                        x.RoleId == adminRole.Id &&
                        x.PermissionId == permission.Id);

            if (!exists)
            {
                context.RolePermissions.Add(
                    new RolePermission
                    {
                        RoleId = adminRole.Id,
                        PermissionId = permission.Id
                    });
            }
        }

        await context.SaveChangesAsync();

        // -------------------------------------------------
        // 6. Associar Administrador ao utilizador admin
        // -------------------------------------------------

        var userRoleExists = await context.UserRoles
            .AnyAsync(
                x =>
                    x.UserId == adminUser.Id &&
                    x.RoleId == adminRole.Id);

        if (!userRoleExists)
        {
            context.UserRoles.Add(
                new UserRole
                {
                    UserId = adminUser.Id,
                    RoleId = adminRole.Id
                });

            await context.SaveChangesAsync();
        }
    }
}