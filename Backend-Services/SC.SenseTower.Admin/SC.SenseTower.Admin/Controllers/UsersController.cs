using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SC.SenseTower.Admin.Constants;
using SC.SenseTower.Admin.Cqrs.FixAccounts;
using SC.SenseTower.Admin.Cqrs.UserBan;
using SC.SenseTower.Admin.Cqrs.UserDelete;
using SC.SenseTower.Admin.Cqrs.UserDetails;
using SC.SenseTower.Admin.Cqrs.UsersPage;
using SC.SenseTower.Admin.Cqrs.UserUnban;
using SC.SenseTower.Admin.Data.Models.Identity;
using SC.SenseTower.Admin.Dto;
using SC.SenseTower.Admin.Models.Users;
using SC.SenseTower.Admin.Settings;
using SC.SenseTower.Common.Attributes;
using SC.SenseTower.Common.Data;

namespace SC.SenseTower.Admin.Controllers
{
    [Authorize]
    public class UsersController : BaseMvcController
    {
        protected override string ActiveMenuItem => MenuItems.MENU_USERS;

        private readonly ServiceEndpointsSettings endpointsSettings;

        public UsersController(
            IMediator mediator,
            ILogger<UsersController> logger,
            IConfiguration configuration) : base(mediator, logger, configuration)
        {
            endpointsSettings = configuration.GetSection(nameof(ServiceEndpointsSettings)).Get<ServiceEndpointsSettings>();
        }

        [HttpGet]
        public IActionResult Index()
        {
            var sorting = new QuerySorting[] { new QuerySorting { Ascending = true, PropertyName = nameof(ApplicationUser.UserName), SortOrder = 0 } };
            var model = new UsersViewModel
            {
                Pagination = new PaginationDto(1, 25, 0, 0),
                Sorting = sorting
            };
            SetIndexViewData();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> GetPage([FromBody] UsersPageRequest command, CancellationToken cancellationToken)
        {
            var data = await Mediator.Send(command, cancellationToken);
            var model = new UsersViewModel
            {
                Items = data.Items,
                Pagination = new PaginationDto(command.CurrentPage, command.PageSize, data.TotalCount, data.Items.Length),
                Sorting = command.Sorting
            };
            return PartialView("_Grid", model);
        }

        [HttpGet, CommonException]
        public async Task<IActionResult> Get(Guid id, CancellationToken cancellationToken)
        {
            var request = new UserDetailsRequest
            {
                UserId = id,
                AccessToken = GetAccessToken(),
                RefreshToken = GetRefreshToken()
            };
            var model = await Mediator.Send(request, cancellationToken);
            return PartialView("_User", model);
        }

        [HttpGet]
        public IActionResult Ban(Guid id)
        {
            var model = new UserBanViewModel { UserId = id };
            return PartialView("_UserBan", model);
        }

        [HttpPost]
        public async Task<IActionResult> Ban(UserBanCommand command, CancellationToken cancellationToken)
        {
            await Mediator.Send(command, cancellationToken);
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> Unban(Guid id, CancellationToken cancellationToken)
        {
            await Mediator.Send(new UserUnbanCommand { UserId = id }, cancellationToken);
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            await Mediator.Send(new UserDeleteCommand
            {
                AccessToken = GetAccessToken(),
                RefreshToken = GetRefreshToken(),
                UserId = id
            }, cancellationToken);
            return Ok();
        }

        /// <summary>
        /// Фикс рассинхронизации таблиц Accounts и ApplicationUsers.
        /// Разовая операция, не использовать.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> FixAccounts(CancellationToken cancellationToken)
        {
            await Mediator.Send(new FixAccountsCommand(), cancellationToken);
            return Ok();
        }

        /// <summary>
        /// Вывод формы загрузки изображения пользователя. Для тестовых целей.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("[controller]/[action]/{id}")]
        public IActionResult Upload(Guid id)
        {
            var token = HttpContext.Request.Cookies["SC.SenseTower.Admin.Token"];
            ViewBag.Token = token;
            var rootUrl = endpointsSettings.ImagesRootUrl;
            if (!rootUrl.EndsWith('/'))
                rootUrl += '/';
            ViewBag.ImagesRootUrl = rootUrl;
            return PartialView("_UploadImage", id);
        }
    }
}
