
using JewelleryWebApplication.Models;
using Microsoft.AspNetCore.Identity;

namespace JewelleryWebApplication.Data
{
    public class DbInitializer
    {
        //public static void SeedData(MyAttendanceWebAppContext context, IServiceProvider serviceProvider)
        //{
        //   // SeedUsers(serviceProvider);
        //}
        //public static async  Task SeedUsers(IServiceProvider serviceProvider, IConfiguration Configuration)
        //{

        //    var RoleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        //    var UserManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        //    string[] roleNames = { "Customer" };
        //    IdentityResult roleResult;
        //    foreach (var roleName in roleNames)
        //    {
        //        // creating the roles and seeding them to the database
        //        var roleExist = await RoleManager.RoleExistsAsync(roleName);
        //        if (!roleExist)
        //        {
        //            roleResult = await RoleManager.CreateAsync(new IdentityRole(roleName));
        //        }
        //    }
        //    // creating a super user who could maintain the web app


        //}
        //public static async Task SeedUsers(IServiceProvider serviceProvider, IConfiguration Configuration)
        //{

        //    var RoleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        //    var UserManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        //    string[] roleNames = { "SuperUser", "Customer" };
        //    IdentityResult roleResult;
        //    foreach (var roleName in roleNames)
        //    {
        //        // creating the roles and seeding them to the database
        //        var roleExist = await RoleManager.RoleExistsAsync(roleName);
        //        if (!roleExist)
        //        {
        //            roleResult = await RoleManager.CreateAsync(new IdentityRole(roleName));
        //        }
        //    }
        //    // creating a super user who could maintain the web app
        //    var poweruser = new ApplicationUser
        //    {
        //        UserName = "info@mkgharejewellers.com",// Configuration.GetSection("AppSettings")["UserEmail"],
        //        Email = "info@mkgharejewellers.com",// Configuration.GetSection("AppSettings")["UserEmail"]
             
        //        EmailConfirmed = true
        //    };
        //    string userPassword = "Password1!";// Configuration.GetSection("AppSettings")["UserPassword"];
        //    var user = await UserManager.FindByEmailAsync("info@mkgharejewellers.com");

        //    if (user == null)
        //    {
        //        var createPowerUser = await UserManager.CreateAsync(poweruser, userPassword);
        //        if (createPowerUser.Succeeded)
        //        {
        //            // here we assign the new user the "Admin" role 
        //            await UserManager.AddToRoleAsync(poweruser, "SuperUser");
        //        }
        //    }
        //}
    }
}

