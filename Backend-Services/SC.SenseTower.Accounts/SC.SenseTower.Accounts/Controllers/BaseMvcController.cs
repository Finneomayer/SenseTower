using MediatR;
using Microsoft.AspNetCore.Mvc;
using SC.SenseTower.Accounts.Constants;
using System.Security.Claims;

namespace SC.SenseTower.Accounts.Controllers
{
    public class BaseMvcController : Controller
    {
        protected readonly IMediator Mediator;
        protected readonly ILogger Logger;
        protected readonly string StaticRootUrl;

        public BaseMvcController(
            IMediator mediator,
            ILogger logger,
            IConfiguration configuration)
        {
            Mediator = mediator;
            Logger = logger;
            StaticRootUrl = configuration[ConfigKeys.STATIC_ROOT_URL];
        }

        protected string GetToken() => HttpContext.Request.Headers["Authorization"].ToString()[7..];

        protected Guid GetUserId() => Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId) ? userId : default;

        protected string GetRole() => User.FindFirstValue(ClaimTypes.Role);
    }
}
