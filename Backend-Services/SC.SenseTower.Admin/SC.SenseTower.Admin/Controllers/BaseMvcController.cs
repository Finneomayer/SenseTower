using MediatR;
using Microsoft.AspNetCore.Mvc;
using SC.SenseTower.Admin.Constants;
using System.Security.Claims;

namespace SC.SenseTower.Admin.Controllers
{
    public abstract class BaseMvcController : Controller
    {
        protected readonly string StaticRootUrl;
        protected abstract string ActiveMenuItem { get; }
        protected readonly IMediator Mediator;
        protected readonly ILogger Logger;

        public BaseMvcController(IMediator mediator, ILogger logger, IConfiguration configuration)
        {
            Mediator = mediator;
            Logger = logger;
            StaticRootUrl = configuration.GetValue<string>(ConfigKeys.STATIC_ROOT_URL);
        }

        protected void SetIndexViewData()
        {
            ViewBag.SelectedMenuItem = ActiveMenuItem;
            ViewBag.StaticRootUrl = StaticRootUrl;
        }

        protected string GetAccessToken() => HttpContext.Request.Cookies["SC.SenseTower.Admin.Token"] ?? string.Empty;

        protected string GetRefreshToken() => HttpContext.Request.Cookies["SC.SenseTower.Admin.Refresh"] ?? string.Empty;

        protected Guid GetUserId() => Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId) ? userId : default;

        protected string GetRole() => User.FindFirstValue(ClaimTypes.Role);
    }
}
