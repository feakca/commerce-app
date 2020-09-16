using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CommerceApp.WebUI.Identity
{
    public static class SeedIdentity
    {
        public static async Task Seed(UserManager<User> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration)
        {
            var userName = configuration["AdminUser:UserName"];
            var password = configuration["AdminUser:Password"];
            var email = configuration["AdminUser:Email"];
            var role = configuration["AdminUser:Role"];

            if (await userManager.FindByNameAsync(userName) == null)
            {
                await roleManager.CreateAsync(new IdentityRole(role));
                await roleManager.CreateAsync(new IdentityRole("Customer"));

                var user = new User()
                {
                    UserName = userName,
                    Email = email,
                    EmailConfirmed = true,
                    FirstName = "Admin",
                    LastName = "Admin"
                };
                var result = await userManager.CreateAsync(user, password);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, role);
                }
            }
        }
    }
}
