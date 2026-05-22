using Microsoft.AspNetCore.Identity;
using SC.SenseTower.Auth.Constants;
using SC.SenseTower.Auth.Extensions;
using SC.SenseTower.Auth.Models;

namespace SC.SenseTower.Auth.Data
{
    public class InitialSeedService
    {
        private readonly ILogger<InitialSeedService> logger;
        private readonly RoleManager<ApplicationRole> roleManager;
        private readonly UserManager<ApplicationUser> userManager;

        public InitialSeedService(
            ILogger<InitialSeedService> logger,
            RoleManager<ApplicationRole> roleManager,
            UserManager<ApplicationUser> userManager)
        {
            this.logger = logger;
            this.roleManager = roleManager;
            this.userManager = userManager;
        }

        private readonly string[] appRoles = new[] { RoleNames.VR_ADMIN, RoleNames.VR_USER };
        private readonly string defaultAdmin = "test_app";

        public async Task SeedData()
        {
            foreach (var appRole in appRoles)
            {
                var role = await roleManager.FindByNameAsync(appRole).ConfigureAwait(false);
                if (role == null)
                {
                    var result = await roleManager.CreateAsync(new ApplicationRole { Name = appRole }).ConfigureAwait(false);
                    if (!result.Succeeded)
                    {
                        logger.LogError($"Error creating default role {appRole}: {result.GetMessages()}");
                    }
                }
            }

            var admin = await userManager.FindByNameAsync(defaultAdmin).ConfigureAwait(false);
            if (admin == null)
            {
                admin = new ApplicationUser
                {
                    UserName = defaultAdmin,
                    Email = "admin@sensecapital.vc"
                };
                IdentityResult result = await userManager.CreateAsync(admin, "Test_app1Test_app1");
                if (result.Succeeded)
                {
                    admin.SecurityStamp = Guid.NewGuid().ToString();
                    await userManager.AddToRoleAsync(admin, RoleNames.VR_ADMIN);
                }
            }
        }
    }
}
