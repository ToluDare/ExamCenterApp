using ExamCenterApp.Database;
using ExamCenterApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;

namespace ExamCenterApp.MiddleWare
{
    public class SeedData
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<Application_Users>>();
            var context = serviceProvider.GetRequiredService<ApplicationDbContext>();
            context.Database.Migrate();

            string[] roleNames = { "ExamManager", "ExamInvigilator" };
            IdentityResult roleResult;

            foreach (var roleName in roleNames)
            {
                var roleExist = await roleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    // Create the roles and seed them to the database
                    roleResult = await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            // Create an admin user who will maintain the web app
            var powerUser = new Application_Users
            {
                UserName = "daret@mymacewan.ca",
                Email = "daret@mymacewan.ca",
                EmailConfirmed = true,
                first_name = "Exam",
                last_name = "Manager",
                PhoneNumber = "1587337784"
            };

            string userPassword = "5DD4C8A8EB0E9937D";
            var user = await userManager.FindByEmailAsync(powerUser.UserName);

            if (user == null)
            {
                var createPowerUser = await userManager.CreateAsync(powerUser, userPassword);
                if (createPowerUser.Succeeded)
                {
                    // Here we assign the new user the "Admin" role
                    await userManager.AddToRoleAsync(powerUser, "ExamManager");
                }
            }
        }
    }
}
