using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SC.SenseTower.Admin.Constants;
using SC.SenseTower.Admin.Cqrs.Lookups;
using SC.SenseTower.Admin.Cqrs.RecallTicket;
using SC.SenseTower.Admin.Cqrs.TicketDetails;
using SC.SenseTower.Admin.Cqrs.TicketsBatchAdd;
using SC.SenseTower.Admin.Cqrs.TicketsPage;
using SC.SenseTower.Admin.Data.Models.Accounts;
using SC.SenseTower.Admin.Dto;
using SC.SenseTower.Admin.Models.Tickets;
using SC.SenseTower.Common.Data;

namespace SC.SenseTower.Admin.Controllers
{
    [Authorize]
    public class TicketsController : BaseMvcController
    {
        protected override string ActiveMenuItem => MenuItems.MENU_TEMP_INVITES;

        public TicketsController(
            IMediator mediator,
            ILogger<TicketsController> logger,
            IConfiguration configuration) : base(mediator, logger, configuration)
        {
        }

        [HttpGet]
        public IActionResult Index()
        {
            var sorting = new QuerySorting[] { new QuerySorting { Ascending = false, PropertyName = nameof(Ticket.CreatedAt), SortOrder = 0 } };
            var model = new TicketsViewModel
            {
                Pagination = new PaginationDto(1, 25, 0, 0),
                Sorting = sorting
            };
            SetIndexViewData();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> GetPage([FromBody] TicketsPageRequest command, CancellationToken cancellationToken)
        {
            var data = await Mediator.Send(command, cancellationToken);
            var model = new TicketsViewModel
            {
                Items = data.Items,
                Pagination = new PaginationDto(command.CurrentPage, command.PageSize, data.TotalCount, data.Items.Length),
                Sorting = command.Sorting
            };
            return PartialView("_Grid", model);
        }

        [HttpGet]
        public async Task<IActionResult> Get(string id, CancellationToken cancellationToken)
        {
            var model = await Mediator.Send(new TicketDetailsRequest { TicketId = id }, cancellationToken);
            return PartialView("_Ticket", model);
        }

        [HttpGet]
        public IActionResult Recall(string id)
        {
            return PartialView("_Recall", id);
        }

        [HttpPost]
        public async Task<IActionResult> Recall(RecallTicketCommand command, CancellationToken cancellationToken)
        {
            await Mediator.Send(command, cancellationToken);
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> BatchAdd(CancellationToken cancellationToken)
        {
            var model = new UserTicketsViewModel
            {
                AvailableUsers = await Mediator.Send(new LookupUsersRequest { Role = RoleNames.VR_ADMIN }, cancellationToken),
                Title = "Билеты администратору"
            };
            return PartialView("_UserTickets", model);
        }

        [HttpPost]
        public async Task<ActionResult<string[]?>> BatchAdd(TicketsBatchAddCommand command, CancellationToken cancellationToken)
        {
            var result = await Mediator.Send(command, cancellationToken);
            return Ok(result);
        }
    }
}
