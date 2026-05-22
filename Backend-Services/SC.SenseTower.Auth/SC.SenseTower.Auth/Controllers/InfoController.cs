using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SC.SenseTower.Auth.Models;
using SC.SenseTower.Auth.Services;

namespace SC.SenseTower.Auth.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class InfoController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<ApplicationRole> roleManager;
        private readonly UsersService usersService;

        public InfoController(
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager,
            UsersService usersService)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.usersService = usersService;
        }

        [HttpGet, AllowAnonymous]
        public async Task<IActionResult> IsLoginFree(string login)
        {
            var user = await userManager.FindByNameAsync(login);
            if (user is { AccessGrantedTo: null })
            {
                return BadRequest("Имя входа уже используется");
            }
            return Ok(true);
        }

        [HttpGet, AllowAnonymous]
        public async Task<IActionResult> IsEmailFree(string email)
        {
            var user = await userManager.FindByEmailAsync(email);
            if (user is { AccessGrantedTo: null })
            {
                return BadRequest("Email уже используется");
            }
            return Ok(true);
        }

        [HttpPost, AllowAnonymous]
        public async Task<IActionResult> CheckLoginOrEmail(string loginOrEmail)
        {
            var user = await userManager.FindByNameAsync(loginOrEmail);
            if (user == null && loginOrEmail.Contains('@'))
            {
                user = await userManager.FindByEmailAsync(loginOrEmail);
            }
            return Ok(user != null);
        }

        [HttpGet, AllowAnonymous]
        public async Task<IActionResult> IsEmailConfirmed(Guid userId)
        {
            var user = await userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                return BadRequest("Пользователь не найден");
            }
            return Ok(user.EmailConfirmed);
        }

        [HttpGet, AllowAnonymous]
        public async Task<IActionResult> IsUserBlocked(Guid userId)
        {
            var user = await userManager.FindByIdAsync(userId.ToString());
            return Ok(user == null ? null : (user.LockoutEnd > DateTime.UtcNow || (user.AccessGrantedTo != null && user.AccessGrantedTo <= DateTime.UtcNow)));
        }

        [HttpGet]
        public IActionResult CheckToken()
        {
            return Ok(true);
        }

        [HttpGet]
        public async Task<IActionResult> ById(Guid id)
        {
            var user = await userManager.FindByIdAsync(id.ToString());
            if (user == null)
                return BadRequest("Пользователь не найден");

            var roleId = user.Roles.First();
            var role = await roleManager.FindByIdAsync(roleId.ToString());
            var result = new UserInfo
            {
                Email = user.Email,
                Login = user.UserName,
                Role = role?.Name,
                UserId = user.Id,
                AccessGrantedTo = user.AccessGrantedTo
            };
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> GetByIds(Guid[] userIds, CancellationToken cancellationToken)
        {
            var result = await usersService.GetByIds(userIds, cancellationToken);
            return Ok(result);
        }
    }
}
