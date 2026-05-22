using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace SC.SenseTower.Common.Controllers
{
    public class BaseController : ControllerBase
    {
        protected readonly IMediator Mediator;
        protected readonly ILogger Logger;

        public BaseController(IMediator mediator, ILogger logger)
        {
            Mediator = mediator;
            Logger = logger;
        }

        protected string GetToken() => HttpContext.Request.Headers["Authorization"].ToString()[7..];

        protected Guid GetUserId() => Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId) ? userId : default;

        protected string GetRole() => User.FindFirstValue(ClaimTypes.Role);
    }
}
