using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SC.SenseTower.Auth.Data;
using SC.SenseTower.Auth.Models;

namespace SC.SenseTower.Auth.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class LookupController : Controller
    {
        private readonly AuthDbContext context;
        private readonly IMapper mapper;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<ApplicationRole> roleManager;

        public LookupController(
            IMapper mapper,
            AuthDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager)
        {
            this.mapper = mapper;
            this.context = context;
            this.userManager = userManager;
            this.roleManager = roleManager;
        }

        [HttpPost]
        public IActionResult Users(Guid[] userIds, string roleName, CancellationToken cancellationToken)
        {
            var query = userManager.Users;
            if ((userIds?.Length ?? 0) > 0)
                query = query.Where(r => userIds.Contains(r.Id));
            if (!string.IsNullOrWhiteSpace(roleName))
            {
                var roleId = roleManager.FindByNameAsync(roleName).GetAwaiter().GetResult()?.Id;
                query = query.Where(r => r.Roles.Any(x => x == roleId));
            }
            var users = query
                .ProjectTo<LookupItemDto>(mapper.ConfigurationProvider)
                .OrderBy(r => r.Name)
                .ToArray();
            return Ok(users);
        }
    }
}
