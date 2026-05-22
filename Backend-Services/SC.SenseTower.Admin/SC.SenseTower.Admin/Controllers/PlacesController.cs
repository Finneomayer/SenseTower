using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SC.SenseTower.Admin.Constants;
using SC.SenseTower.Admin.Cqrs.AvailableImages;
using SC.SenseTower.Admin.Cqrs.AvailableSpaces;
using SC.SenseTower.Admin.Cqrs.AvailableUsers;
using SC.SenseTower.Admin.Cqrs.Lookups;
using SC.SenseTower.Admin.Cqrs.Place;
using SC.SenseTower.Admin.Cqrs.PlaceCreate;
using SC.SenseTower.Admin.Cqrs.PlaceDelete;
using SC.SenseTower.Admin.Cqrs.PlaceImagesUpdate;
using SC.SenseTower.Admin.Cqrs.PlacesPage;
using SC.SenseTower.Admin.Cqrs.PlaceUpdate;
using SC.SenseTower.Admin.Dto;
using SC.SenseTower.Admin.Dto.Places;
using SC.SenseTower.Admin.Models.Places;
using SC.SenseTower.Common.Attributes;
using SC.SenseTower.Common.Data;

namespace SC.SenseTower.Admin.Controllers
{
    [Authorize]
    public class PlacesController : BaseMvcController
    {
        public PlacesController(
            IMediator mediator,
            ILogger<PlacesController> logger,
            IConfiguration configuration) : base(mediator, logger, configuration)
        {
        }

        protected override string ActiveMenuItem => MenuItems.MENU_PLACES;

        [HttpGet]
        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            var sorting = new QuerySorting[] { new QuerySorting { Ascending = true, PropertyName = nameof(PlacesGridItemDto.PlaceName), SortOrder = 0 } };
            var model = new PlacesViewModel
            {
                Pagination = new PaginationDto(1, 25, 0, 0),
                Sorting = sorting
            };
            model.Filter.AvailableUsers = await Mediator.Send(new LookupUsersRequest { Role = RoleNames.VR_ADMIN, Eq = false }, cancellationToken);
            model.Filter.AvailableSpaces = await Mediator.Send(new AvailableSpacesRequest { AccessToken = GetAccessToken(), RefreshToken = GetRefreshToken() }, cancellationToken);
            SetIndexViewData();
            return View(model);
        }

        [HttpPost, CommonException]
        public async Task<IActionResult> GetPage([FromBody] PlacesPageRequest command, CancellationToken cancellationToken)
        {
            command.AccessToken = GetAccessToken();
            command.RefreshToken = GetRefreshToken();
            var data = await Mediator.Send(command, cancellationToken);
            var model = new PlacesViewModel
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
            var model = await Mediator.Send(new PlaceRequest
            {
                Id = id,
                AccessToken = GetAccessToken(),
                RefreshToken = GetRefreshToken()
            }, cancellationToken);
            return PartialView("_Item", model);
        }

        [HttpGet, CommonException]
        public async Task<IActionResult> Create(CancellationToken cancellationToken)
        {
            var model = new PlaceDto
            {
                AvailableImages = await Mediator.Send(new AvailableImagesRequest { AccessToken = GetAccessToken(), RefreshToken = GetRefreshToken() }, cancellationToken),
                AvailableSpaces = await Mediator.Send(new AvailableSpacesRequest { AccessToken = GetAccessToken(), RefreshToken = GetRefreshToken(), SpaceType = Common.Enums.SpaceType.MySpace }, cancellationToken),
                AvailableUsers = await Mediator.Send(new AvailableUsersRequest { AccessToken = GetAccessToken(), RefreshToken = GetRefreshToken(), RoleName = RoleNames.VR_USER }, cancellationToken)
            };
            return PartialView("_Create", model);
        }

        [HttpPost, CommonException]
        public async Task<IActionResult> Create(PlaceCreateCommand command, CancellationToken cancellationToken)
        {
            command.AccessToken = GetAccessToken();
            command.RefreshToken = GetRefreshToken();
            await Mediator.Send(command, cancellationToken);
            return Ok();
        }

        [HttpPost, CommonException]
        public async Task<IActionResult> Update(PlaceUpdateCommand command, CancellationToken cancellationToken)
        {
            command.AccessToken = GetAccessToken();
            command.RefreshToken = GetRefreshToken();
            await Mediator.Send(command, cancellationToken);
            return Ok();
        }

        [HttpPost, CommonException]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            var command = new PlaceDeleteCommand
            {
                Id = id,
                AccessToken = GetAccessToken(),
                RefreshToken = GetRefreshToken()
            };
            await Mediator.Send(command, cancellationToken);
            return Ok();
        }

        [HttpGet, CommonException]
        public async Task<IActionResult> Images(Guid id, CancellationToken cancellationToken)
        {
            var place = await Mediator.Send(new PlaceRequest { Id = id, AccessToken = GetAccessToken(), RefreshToken = GetRefreshToken() }, cancellationToken);
            var model = new PlaceImagesViewModel
            {
                PlaceId = id,
                Images = place.Images ?? new Dictionary<int, Dto.Images.ImageInfoDto>(),
                AvailableImages = place.AvailableImages
            };
            SetIndexViewData();
            return PartialView("_Images", model);
        }

        [HttpPost, CommonException]
        public async Task<IActionResult> Images(Guid id, PlaceImageSaveDto[] images, CancellationToken cancellationToken)
        {
            var command = new PlaceImagesUpdateCommand
            {
                PlaceId = id,
                Images = images,
                AccessToken = GetAccessToken(),
                RefreshToken = GetRefreshToken()
            };
            await Mediator.Send(command, cancellationToken);
            return Ok();
        }
    }
}
