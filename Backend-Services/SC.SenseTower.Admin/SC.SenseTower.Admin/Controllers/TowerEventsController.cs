using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SC.SenseTower.Admin.Constants;
using SC.SenseTower.Admin.Cqrs.AvailableImages;
using SC.SenseTower.Admin.Cqrs.AvailableSpaces;
using SC.SenseTower.Admin.Cqrs.TowerEvent;
using SC.SenseTower.Admin.Cqrs.TowerEventAddTickets;
using SC.SenseTower.Admin.Cqrs.TowerEventCreate;
using SC.SenseTower.Admin.Cqrs.TowerEventDelete;
using SC.SenseTower.Admin.Cqrs.TowerEventsPage;
using SC.SenseTower.Admin.Cqrs.TowerEventTickets;
using SC.SenseTower.Admin.Cqrs.TowerEventUpdate;
using SC.SenseTower.Admin.Dto;
using SC.SenseTower.Admin.Dto.TowerEvents;
using SC.SenseTower.Admin.Models.TowerEvents;
using SC.SenseTower.Common.Attributes;
using SC.SenseTower.Common.Data;
using System.Threading;

namespace SC.SenseTower.Admin.Controllers
{
    [Authorize]
    public class TowerEventsController : BaseMvcController
    {
        public TowerEventsController(
            IMediator mediator,
            ILogger<TowerEventsController> logger,
            IConfiguration configuration) : base(mediator, logger, configuration)
        {
        }

        protected override string ActiveMenuItem => MenuItems.MENU_TOWER_EVENTS;

        [HttpGet]
        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            var sorting = new QuerySorting[] { new QuerySorting { Ascending = false, PropertyName = nameof(TowerEventGridItemDto.Date), SortOrder = 0 } };
            var model = new TowerEventsViewModel
            {
                Pagination = new PaginationDto(1, 25, 0, 0),
                Sorting = sorting
            };
            model.Filter.AvailableSpaces = await Mediator.Send(new AvailableSpacesRequest { AccessToken = GetAccessToken(), RefreshToken = GetRefreshToken() }, cancellationToken);
            SetIndexViewData();
            return View(model);
        }

        [HttpPost, CommonException]
        public async Task<IActionResult> GetPage([FromBody] TowerEventsPageRequest command, CancellationToken cancellationToken)
        {
            command.AccessToken = GetAccessToken();
            command.RefreshToken = GetRefreshToken();
            var data = await Mediator.Send(command, cancellationToken);
            var model = new TowerEventsViewModel
            {
                Items = data.Items,
                Pagination = new PaginationDto(command.Page, command.PageSize, data.TotalCount, data.Items.Length),
                Sorting = command.Sorting,
            };
            return PartialView("_Grid", model);
        }

        [HttpGet, CommonException]
        public async Task<IActionResult> Get(Guid id, CancellationToken cancellationToken)
        {
            var model = await Mediator.Send(new TowerEventRequest
            {
                Id = id,
                AccessToken = GetAccessToken(),
                RefreshToken = GetRefreshToken()
            }, cancellationToken);
            SetIndexViewData();
            return PartialView("_Item", model);
        }

        [HttpGet, CommonException]
        public async Task<IActionResult> Create(CancellationToken cancellationToken)
        {
            var model = new TowerEventDto
            {
                From = DateTimeOffset.Now.AddDays(1),
                UpTo = DateTimeOffset.Now.AddDays(1).AddHours(2),
                AvailableImages = await Mediator.Send(new AvailableImagesRequest { AccessToken = GetAccessToken(), RefreshToken = GetRefreshToken() }, cancellationToken),
                AvailableSpaces = await Mediator.Send(new AvailableSpacesRequest { AccessToken = GetAccessToken(), RefreshToken = GetRefreshToken() }, cancellationToken)
            };
            SetIndexViewData();
            return PartialView("_Create", model);
        }

        [HttpPost, CommonException]
        public async Task<IActionResult> Create(TowerEventCreateCommand command, CancellationToken cancellationToken)
        {
            command.AccessToken = GetAccessToken();
            command.RefreshToken = GetRefreshToken();
            await Mediator.Send(command, cancellationToken);
            return Ok();
        }

        [HttpPost, CommonException]
        public async Task<IActionResult> Update(TowerEventUpdateCommand command, CancellationToken cancellationToken)
        {
            command.AccessToken = GetAccessToken();
            command.RefreshToken = GetRefreshToken();
            await Mediator.Send(command, cancellationToken);
            return Ok();
        }

        [HttpPost, CommonException]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            var command = new TowerEventDeleteCommand
            {
                EventId = id,
                AccessToken = GetAccessToken(),
                RefreshToken = GetRefreshToken()
            };
            await Mediator.Send(command, cancellationToken);
            return Ok();
        }

        #region Билеты

        [HttpGet, CommonException]
        public async Task<IActionResult> AddTickets(Guid id, CancellationToken cancellationToken)
        {
            var model = new TowerEventAddTicketsViewModel { EventId = id };
            return PartialView("_AddTickets", model);
        }

        [HttpPost, CommonException]
        public async Task<IActionResult> AddTickets(TowerEventAddTicketsCommand command, CancellationToken cancellationToken)
        {
            command.AccessToken = GetAccessToken();
            command.RefreshToken = GetRefreshToken();
            await Mediator.Send(command, cancellationToken);
            return Ok();
        }

        [HttpGet, CommonException]
        public async Task<IActionResult> ViewTickets(Guid id, CancellationToken cancellationToken)
        {
            var request = new TowerEventTicketsRequest
            {
                Id = id,
                AccessToken = GetAccessToken(),
                RefreshToken = GetRefreshToken()
            };
            var model = new TowerEventSoldTicketsViewModel
            {
                Tickets = await Mediator.Send(request, cancellationToken)
            };
            return PartialView("_SoldTickets", model);
        }

        #endregion
    }
}
