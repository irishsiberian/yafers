using Microsoft.AspNetCore.Identity;
using Yafers.Web.Consts;
using Yafers.Web.Data.Entities;

namespace Yafers.Web.Data
{
    public static class IdentitySeeder
    {
        public static async Task SeedRolesAndAdminAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            // Define roles to seed
            var roles = new[] { RoleNames.Admin, RoleNames.Teacher, RoleNames.Dancer, RoleNames.Organiser, RoleNames.Adjudicator, RoleNames.DancerParent };

            // Seed roles
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    var res = await roleManager.CreateAsync(new ApplicationRole{Name = role});

                    if (!res.Succeeded)
                    {
                        throw new Exception("Failed to create role " + role + ": " + string.Join(", ", res.Errors));
                    }
                }
            }

            // Define the admin user details
            var adminEmail = "irishsiberian@gmail.com";
            var adminPassword = "a423CBsn!";

            // Check if the admin user already exists
            var userExist = await userManager.FindByEmailAsync(adminEmail);
            if (userExist != null)
            {
                await userManager.DeleteAsync(userExist);
            }
            userExist = await userManager.FindByEmailAsync(adminEmail);
            if (userExist == null)
            {
                var adminUser = new ApplicationUser
                {
                    UserName = "admin",
                    Email = adminEmail,
                    PhoneNumber = "",
                    EmailConfirmed = true
                };

                // Create the admin user
                var result = await userManager.CreateAsync(adminUser, adminPassword);
                if (result.Succeeded)
                {
                    // Assign the Admin role to the user
                    await userManager.AddToRoleAsync(adminUser, RoleNames.Admin);
                }
                else
                {
                    throw new Exception("Failed to create the admin user: " + string.Join(", ", result.Errors));
                }
            }
        }
    }
}
