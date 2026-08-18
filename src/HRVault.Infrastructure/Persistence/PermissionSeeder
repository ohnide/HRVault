using HRVault.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HRVault.Infrastructure.Persistence;

public static class PermissionSeeder
{
    public static async Task SeedAsync(
        ApplicationDbContext context)
    {
        var permissions = new[]
        {
            new Permission
            {
                Code = "Employees.View",
                Name = "Ver funcionários",
                Description = "Permite consultar funcionários."
            },
            new Permission
            {
                Code = "Employees.Create",
                Name = "Criar funcionários",
                Description = "Permite criar funcionários."
            },
            new Permission
            {
                Code = "Employees.Update",
                Name = "Editar funcionários",
                Description = "Permite editar funcionários."
            },
            new Permission
            {
                Code = "Employees.Delete",
                Name = "Eliminar funcionários",
                Description = "Permite eliminar funcionários."
            },
			
			new Permission
			{
				Code = "Documents.View",
				Name = "Ver documentos",
				Description = "Permite consultar e descarregar documentos."
			},
			new Permission
			{
				Code = "Documents.Upload",
				Name = "Carregar documentos",
				Description = "Permite carregar documentos de funcionários."
			},
			new Permission
			{
				Code = "Documents.Delete",
				Name = "Eliminar documentos",
				Description = "Permite eliminar documentos de funcionários."
			},
			new Permission
			{
				Code = "Documents.ManageTypes",
				Name = "Gerir tipos de documentos",
				Description = "Permite criar, editar e eliminar tipos de documentos."
			},
			
			new Permission
			{
				Code = "Absences.View",
				Name = "Ver ausências",
				Description = "Permite consultar ausências."
			},
			new Permission
			{
				Code = "Absences.Create",
				Name = "Criar ausências",
				Description = "Permite registar ausências de funcionários."
			},
			new Permission
			{
				Code = "Absences.Update",
				Name = "Editar ausências",
				Description = "Permite editar e alterar o estado de ausências."
			},
			new Permission
			{
				Code = "Absences.Delete",
				Name = "Eliminar ausências",
				Description = "Permite eliminar ausências."
			},
			new Permission
			{
				Code = "Absences.ManageTypes",
				Name = "Gerir tipos de ausência",
				Description = "Permite criar, editar e eliminar tipos de ausência."
			},

            new Permission
            {
                Code = "Departments.View",
                Name = "Ver departamentos",
                Description = "Permite consultar departamentos."
            },
            new Permission
            {
                Code = "Departments.Create",
                Name = "Criar departamentos",
                Description = "Permite criar departamentos."
            },
            new Permission
            {
                Code = "Departments.Update",
                Name = "Editar departamentos",
                Description = "Permite editar departamentos."
            },
            new Permission
            {
                Code = "Departments.Delete",
                Name = "Eliminar departamentos",
                Description = "Permite eliminar departamentos."
            },

            new Permission
            {
                Code = "Positions.View",
                Name = "Ver cargos",
                Description = "Permite consultar cargos."
            },
            new Permission
            {
                Code = "Positions.Create",
                Name = "Criar cargos",
                Description = "Permite criar cargos."
            },
            new Permission
            {
                Code = "Positions.Update",
                Name = "Editar cargos",
                Description = "Permite editar cargos."
            },
            new Permission
            {
                Code = "Positions.Delete",
                Name = "Eliminar cargos",
                Description = "Permite eliminar cargos."
            },

            new Permission
            {
                Code = "Users.View",
                Name = "Ver utilizadores",
                Description = "Permite consultar utilizadores."
            },
            new Permission
            {
                Code = "Users.Create",
                Name = "Criar utilizadores",
                Description = "Permite criar utilizadores."
            },
            new Permission
            {
                Code = "Users.Update",
                Name = "Editar utilizadores",
                Description = "Permite editar utilizadores."
            },
            new Permission
            {
                Code = "Users.Delete",
                Name = "Eliminar utilizadores",
                Description = "Permite eliminar utilizadores."
            },
			new Permission
            {
                Code = "Users.ResetPassword",
                Name = "Reset Password",
                Description = "Permite fazer reset password."
            },

            new Permission
            {
                Code = "Roles.View",
                Name = "Ver funções",
                Description = "Permite consultar funções."
            },
            new Permission
            {
                Code = "Roles.Create",
                Name = "Criar funções",
                Description = "Permite criar funções."
            },
            new Permission
            {
                Code = "Roles.Update",
                Name = "Editar funções",
                Description = "Permite editar funções."
            },
            new Permission
            {
                Code = "Roles.Delete",
                Name = "Eliminar funções",
                Description = "Permite eliminar funções."
            }
        };

        var existingCodes = await context.Permissions
            .Select(x => x.Code)
            .ToListAsync();

        var newPermissions = permissions
            .Where(x => !existingCodes.Contains(x.Code))
            .ToList();

        if (newPermissions.Count == 0)
            return;

        await context.Permissions.AddRangeAsync(newPermissions);

        await context.SaveChangesAsync();
    }
}