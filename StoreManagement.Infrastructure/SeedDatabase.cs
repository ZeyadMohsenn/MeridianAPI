﻿using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using StoreManagement.Domain.Entities;

namespace StoreManagement.Infrastructure;

public class SeedDatabase
{

    public const string SuperAdminId = "4B801076-1C33-4050-AEF3-DCB5CDD0B5F4";
    public static async Task Initialize(IServiceProvider serviceProvider)
    {
        try
        {
            var context = serviceProvider.GetRequiredService<StoreDbContext>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            await context.Database.MigrateAsync();

            await SeedUsers(context, userManager).ConfigureAwait(false);
            await SeedRoles(serviceProvider).ConfigureAwait(false);


            var savedRows = await context.SaveChangesAsync();
        }
        catch (Exception ex) { throw; }
    }

    private static async Task SeedUsers(StoreDbContext context, UserManager<ApplicationUser> userManager)
    {
        if (!await context.Users.AnyAsync())
        {
            // Seed Admin user
            var adminUser = new ApplicationUser()
            {
                Id = Guid.Parse(SuperAdminId),

                Full_Name = "Admin",
                UserName = "admin",
                NormalizedUserName = "ADMIN",
                Email = "admin@StoreManagement.com",
                NormalizedEmail = "ADMIN@StoreManagement.COM",
                EmailConfirmed = true,
                LockoutEnabled = false,
                TwoFactorEnabled = false,
                AccessFailedCount = 0,
                SecurityStamp = Guid.NewGuid().ToString(),
                Is_Active = true,
                Is_Deleted = false,
                Creation_Time = DateTime.Now,
            };

            var adminSaved = await userManager.CreateAsync(adminUser, "AdminP@55w0rd");

            if (adminSaved.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }

            // Seed Cashier user
            var cashierUser = new ApplicationUser()
            {
                Full_Name = "Cashier",
                UserName = "cashier",
                NormalizedUserName = "CASHIER",
                Email = "cashier@StoreManagement.com",
                NormalizedEmail = "CASHIER@StoreManagement.COM",
                EmailConfirmed = true,
                LockoutEnabled = false,
                TwoFactorEnabled = false,
                AccessFailedCount = 0,
                SecurityStamp = Guid.NewGuid().ToString(),
                Is_Active = true,
                Is_Deleted = false,
                Creation_Time = DateTime.Now,
            };

            var cashierSaved = await userManager.CreateAsync(cashierUser, "CashierP@55w0rd");

            if (cashierSaved.Succeeded)
            {
                await userManager.AddToRoleAsync(cashierUser, "Cashier");
            }

        }

    }
    private static async Task SeedRoles(IServiceProvider serviceProvider)
    {
        var roleManager = serviceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
        if (!await roleManager.Roles.AnyAsync())
        {
            await roleManager.CreateAsync(new ApplicationRole { Name = "Admin" });
            await roleManager.CreateAsync(new ApplicationRole { Name = "Cashier" });
        }
    }



}

