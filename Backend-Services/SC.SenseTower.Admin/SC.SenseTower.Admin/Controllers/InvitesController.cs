using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SC.SenseTower.Admin.Constants;
using SC.SenseTower.Admin.Cqrs.InviteDetails;
using SC.SenseTower.Admin.Cqrs.InvitesBatchAdd;
using SC.SenseTower.Admin.Cqrs.InvitesPage;
using SC.SenseTower.Admin.Cqrs.Lookups;
using SC.SenseTower.Admin.Cqrs.RecallInvite;
using SC.SenseTower.Admin.Data.Models.Accounts;
using SC.SenseTower.Admin.Dto;
using SC.SenseTower.Admin.Models.Invites;
using SC.SenseTower.Common.Data;

namespace SC.SenseTower.Admin.Controllers
{
    [Authorize]
    public class InvitesController : BaseMvcController
    {
        protected override string ActiveMenuItem => MenuItems.MENU_INVITES;

        public InvitesController(
            IMediator mediator,
            ILogger<InvitesController> logger,
            IConfiguration configuration) : base(mediator, logger, configuration)
        {
        }

        [HttpGet]
        public IActionResult Index()
        {
            var sorting = new QuerySorting[] { new QuerySorting { Ascending = false, PropertyName = nameof(Invite.CreatedAt), SortOrder = 0 } };
            var model = new InvitesViewModel
            {
                Pagination = new PaginationDto(1, 25, 0, 0),
                Sorting = sorting
            };
            SetIndexViewData();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> GetPage([FromBody] InvitesPageRequest command, CancellationToken cancellationToken)
        {
            var data = await Mediator.Send(command, cancellationToken);
            var model = new InvitesViewModel
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
            var model = await Mediator.Send(new InviteDetailsRequest { InviteId = id }, cancellationToken);
            return PartialView("_Invite", model);
        }

        [HttpGet]
        public IActionResult Recall(string id)
        {
            return PartialView("_Recall", id);
        }

        [HttpPost]
        public async Task<IActionResult> Recall(RecallInviteCommand command, CancellationToken cancellationToken)
        {
            await Mediator.Send(command, cancellationToken);
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> AdminBatchAdd(CancellationToken cancellationToken)
        {
            var model = new UserInvitesViewModel
            {
                AvailableUsers = await Mediator.Send(new LookupUsersRequest { Role = RoleNames.VR_ADMIN }, cancellationToken),
                Title = "Приглашения администратору"
            };
            return PartialView("_UserInvites", model);
        }

        [HttpGet]
        public async Task<IActionResult> UserBatchAdd(CancellationToken cancellationToken)
        {
            var model = new UserInvitesViewModel
            {
                AvailableUsers = await Mediator.Send(new LookupUsersRequest { Role = RoleNames.VR_ADMIN, Eq = false }, cancellationToken),
                Quantity = 1,
                Title = "Приглашения пользователю"
            };
            return PartialView("_UserInvites", model);
        }

        [HttpPost]
        public async Task<IActionResult> UserBatchAdd(InvitesBatchAddCommand command, CancellationToken cancellationToken)
        {
            command.AuthorId = GetUserId();
            var result = await Mediator.Send(command, cancellationToken);
            return Ok(result);
        }
    }
}
