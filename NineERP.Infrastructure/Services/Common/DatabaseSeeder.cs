using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using NineERP.Application.Constants.Role;
using NineERP.Application.Constants.User;
using NineERP.Domain.Entities;
using NineERP.Domain.Entities.Identity;
using NineERP.Domain.Enums;
using NineERP.Infrastructure.Contexts;
using System.Security.Claims;
using NineERP.Application.Interfaces.Common;

namespace NineERP.Infrastructure.Services.Common
{
    public class DatabaseSeeder(
        UserManager<AppUser> userManager,
        RoleManager<AppRole> roleManager,
        ILogger<DatabaseSeeder> logger) : IDatabaseSeeder
    {
        public void Initialize()
        {
            AddAdministrator();
            AddDefaultAdminUser();
            SeedEmailTemplates();
        }

        private void AddAdministrator()
        {
            Task.Run(async () =>
            {
                var sysAdminRole = new AppRole(RoleConstants.SuperAdmin, "Administrator role with full permissions");
                var sysAdminRoleInDb = await roleManager.FindByNameAsync(RoleConstants.SuperAdmin);
                if (sysAdminRoleInDb == null)
                {
                    await roleManager.CreateAsync(sysAdminRole);
                    logger.LogInformation("Seeded SuperAdmin Role.");

                    // Gán toàn bộ permission cho SuperAdmin (sử dụng phân loại để dễ nhìn)
                    var allPermissions = new List<string>
                    {
                        // Dashboard
                        PermissionValue.Dashboard.View,

                        // Report
                        PermissionValue.Report.View,
                        PermissionValue.Report.Add,
                        PermissionValue.Report.Update,
                        PermissionValue.Report.Delete,

                        // System
                        PermissionValue.System.View,

                        // Users
                        PermissionValue.Users.View,
                        PermissionValue.Users.Add,
                        PermissionValue.Users.Update,
                        PermissionValue.Users.Delete,

                        // Roles
                        PermissionValue.Roles.View,
                        PermissionValue.Roles.Add,
                        PermissionValue.Roles.Update,
                        PermissionValue.Roles.Delete
                    };

                    foreach (var permission in allPermissions)
                    {
                        await roleManager.AddClaimAsync(sysAdminRole, new Claim(PermissionValue.PermissionType, permission));
                    }
                }

                var superUser = new AppUser
                {
                    FullName = "NinePlus Solution",
                    Email = "superadmin@gmail.com",
                    UserName = "superAdmin",
                    PhoneNumber = "0868486885",
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true,
                    LockoutEnabled= false,
                    CreatedOn = DateTime.Now
                };

                var superUserInDb = await userManager.FindByEmailAsync(superUser.Email);
                if (superUserInDb == null)
                {
                    await userManager.CreateAsync(superUser, UserConstants.DefaultPassword);
                    var result = await userManager.AddToRoleAsync(superUser, RoleConstants.SuperAdmin);

                    if (result.Succeeded)
                        logger.LogInformation("Seeded Default SuperAdmin User.");
                    else
                        LogErrors(result.Errors);
                }
            }).GetAwaiter().GetResult();
        }

        private void AddDefaultAdminUser()
        {
            Task.Run(async () =>
            {
                var adminRole = new AppRole(RoleConstants.Admin, "Admin role for company use");
                var adminRoleInDb = await roleManager.FindByNameAsync(RoleConstants.Admin);
                if (adminRoleInDb == null)
                {
                    await roleManager.CreateAsync(adminRole);
                    logger.LogInformation("Seeded Admin Role.");

                    var adminPermissions = new[]
                    {
                        // Dashboard
                        PermissionValue.Dashboard.View,

                        // Report
                        PermissionValue.Report.View,
                        PermissionValue.Report.Add,
                        PermissionValue.Report.Update,
                        PermissionValue.Report.Delete,

                        // System
                        PermissionValue.System.View,

                        // Users
                        PermissionValue.Users.View,
                        PermissionValue.Users.Add,
                        PermissionValue.Users.Update,
                        PermissionValue.Users.Delete,

                        // Roles
                        PermissionValue.Roles.View,
                        PermissionValue.Roles.Add,
                        PermissionValue.Roles.Update,
                        PermissionValue.Roles.Delete
                    };

                    foreach (var permission in adminPermissions)
                    {
                        await roleManager.AddClaimAsync(adminRole, new Claim(PermissionValue.PermissionType, permission));
                    }
                }

                var adminUser = new AppUser
                {
                    FullName = "Client Admin",
                    Email = "admin@company.com",
                    UserName = "admin",
                    PhoneNumber = "0123456789",
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true,
                    LockoutEnabled = false,
                    CreatedOn = DateTime.Now
                };

                var adminInDb = await userManager.FindByEmailAsync(adminUser.Email);
                if (adminInDb == null)
                {
                    await userManager.CreateAsync(adminUser, UserConstants.DefaultPassword);
                    var result = await userManager.AddToRoleAsync(adminUser, RoleConstants.Admin);

                    if (result.Succeeded)
                        logger.LogInformation("Seeded Default Admin User.");
                    else
                        LogErrors(result.Errors);
                }
            }).GetAwaiter().GetResult();
        }

        private void LogErrors(IEnumerable<IdentityError> errors)
        {
            foreach (var error in errors)
            {
                logger.LogError(error.Description);
            }
        }
        private void SeedEmailTemplates()
        {
            Task.Run(async () =>
            {
                var context = userManager.Users.FirstOrDefault()?.CreatedBy != null
                 ? (ApplicationDbContext?)userManager.GetType().GetProperty("Context", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.GetValue(userManager)
                 : null;

                if (context == null)
                {
                    logger.LogWarning("DbContext could not be resolved for seeding email templates.");
                    return;
                }
                if (!context.EmailTemplates.Any())
                {
                    var now = DateTime.UtcNow;
                    var templates = new List<EmailTemplate>
                    {
                        new()
                        {
                            Code = "CONFIRM_EMAIL",
                            Language = "en",
                            Subject = "Confirm your email",
                            Body = "<p>Please confirm your account by clicking <a href='{{Link}}'>here</a>.</p>",
                            CreatedOn = now,
                            CreatedBy = "System"
                        },
                        new()
                        {
                            Code = "CONFIRM_EMAIL",
                            Language = "vi",
                            Subject = "Xác nhận email của bạn",
                            Body = "<p>Vui lòng xác nhận tài khoản của bạn bằng cách nhấn vào <a href='{{Link}}'>đây</a>.</p>",
                            CreatedOn = now,
                            CreatedBy = "System"
                        }
                    };

                    await context.EmailTemplates.AddRangeAsync(templates);
                    await context.SaveChangesAsync();

                    logger.LogInformation("✅ Seeded Email Templates.");
                }
            }).GetAwaiter().GetResult();
        }

    }
}
