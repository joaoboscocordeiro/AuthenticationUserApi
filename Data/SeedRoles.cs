using Microsoft.AspNetCore.Identity;

namespace AuthenticationUserApi.Data
{
    public class SeedRoles
    {
        public static async Task CreateRolesAsync(IServiceProvider serviceProvider)
        {
            var reloManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            string[] roleNames = { "Admin", "User" };

            foreach (var role in roleNames) 
            {
                if (!await reloManager.RoleExistsAsync(role)) 
                {
                    await reloManager.CreateAsync(new IdentityRole(role));
                }
            }
        }
    }
}
