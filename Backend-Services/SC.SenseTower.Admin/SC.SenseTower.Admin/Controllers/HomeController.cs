using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SC.SenseTower.Admin.Constants;
using SC.SenseTower.Admin.Cqrs.UserStatistics;
using SC.SenseTower.Admin.Models;
using SC.SenseTower.Admin.Models.Home;
using System.Diagnostics;

namespace SC.SenseTower.Admin.Controllers
{
    [Authorize]
    public class HomeController : BaseMvcController
    {
        protected override string ActiveMenuItem => MenuItems.MENU_HOME;

        public HomeController(
            IMediator mediator,
            ILogger<HomeController> logger,
            IConfiguration configuration) : base(mediator, logger, configuration)
        {
        }

        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            var model = new HomeViewModel
            {
                UserStatistics = await Mediator.Send(new UserStatisticsQuery(), cancellationToken)
            };
            SetIndexViewData();
            return View(model);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}