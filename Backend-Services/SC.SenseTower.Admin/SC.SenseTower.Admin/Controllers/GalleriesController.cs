using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SC.SenseTower.Admin.Constants;
using SC.SenseTower.Admin.Cqrs.AvailableImages;
using SC.SenseTower.Admin.Cqrs.AvailableSpaces;
using SC.SenseTower.Admin.Cqrs.GalleriesPage;
using SC.SenseTower.Admin.Cqrs.Gallery;
using SC.SenseTower.Admin.Cqrs.GalleryCreate;
using SC.SenseTower.Admin.Cqrs.GalleryDelete;
using SC.SenseTower.Admin.Cqrs.GalleryImageAdd;
using SC.SenseTower.Admin.Cqrs.GalleryImageRemove;
using SC.SenseTower.Admin.Cqrs.GalleryImages;
using SC.SenseTower.Admin.Cqrs.GalleryImagesSave;
using SC.SenseTower.Admin.Cqrs.GalleryUpdate;
using SC.SenseTower.Admin.Cqrs.ImagesGetUsers;
using SC.SenseTower.Admin.Dto;
using SC.SenseTower.Admin.Dto.Galleries;
using SC.SenseTower.Admin.Models.Galleries;
using SC.SenseTower.Common.Attributes;
using SC.SenseTower.Common.Data;

namespace SC.SenseTower.Admin.Controllers
{
    [Authorize]
    public class GalleriesController : BaseMvcController
    {
        public GalleriesController(
            IMediator mediator,
            ILogger<GalleriesController> logger,
            IConfiguration configuration) : base(mediator, logger, configuration)
        {
        }

        protected override string ActiveMenuItem => MenuItems.MENU_GALLERIES;

        [HttpGet]
        public IActionResult Index()
        {
            var sorting = new QuerySorting[] { new QuerySorting { Ascending = true, PropertyName = nameof(GalleryGridItemDto.Name), SortOrder = 0 } };
            var model = new GalleriesViewModel
            {
                Pagination = new PaginationDto(1, 25, 0, 0),
                Sorting = sorting
            };
            SetIndexViewData();
            return View(model);
        }

        [HttpPost, CommonException]
        public async Task<IActionResult> GetPage([FromBody] GalleriesPageRequest command, CancellationToken cancellationToken)
        {
            command.AccessToken = GetAccessToken();
            var data = await Mediator.Send(command, cancellationToken);
            var model = new GalleriesViewModel
            {
                Items = data.Items,
                Pagination = new PaginationDto(command.Page, command.PageSize, data.TotalCount, data.Items.Length),
                Sorting = command.Sorting
            };
            return PartialView("_Grid", model);
        }

        [HttpGet, CommonException]
        public async Task<IActionResult> Get(Guid id, CancellationToken cancellationToken)
        {
            var model = await Mediator.Send(new GalleryRequest
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
            var model = new GalleryDto
            {
                IsVisible = true,
                AvailableImages = await Mediator.Send(new AvailableImagesRequest { AccessToken = GetAccessToken(), RefreshToken = GetRefreshToken() }, cancellationToken),
                AvailableSpaces = await Mediator.Send(new AvailableSpacesRequest { AccessToken = GetAccessToken(), RefreshToken = GetRefreshToken(), SpaceType = Common.Enums.SpaceType.MyGallery }, cancellationToken)
            };
            return PartialView("_Create", model);
        }

        [HttpPost, CommonException]
        public async Task<IActionResult> Create(GalleryCreateCommand command, CancellationToken cancellationToken)
        {
            command.AccessToken = GetAccessToken();
            command.RefreshToken = GetRefreshToken();
            await Mediator.Send(command, cancellationToken);
            return Ok();
        }

        [HttpPost, CommonException]
        public async Task<IActionResult> Update(GalleryUpdateCommand command, CancellationToken cancellationToken)
        {
            command.AccessToken = GetAccessToken();
            command.RefreshToken = GetRefreshToken();
            await Mediator.Send(command, cancellationToken);
            return Ok();
        }

        [HttpPost, CommonException]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            var command = new GalleryDeleteCommand
            {
                GalleryId = id,
                AccessToken = GetAccessToken(),
                RefreshToken = GetRefreshToken()
            };
            await Mediator.Send(command, cancellationToken);
            return Ok();
        }

        [HttpGet, CommonException]
        public async Task<IActionResult> Images(Guid id, CancellationToken cancellationToken)
        {
            var model = new GalleryImagesViewModel
            {
                GalleryId = id,
                Images = await Mediator.Send(new GalleryImagesRequest { GalleryId = id, AccessToken = GetAccessToken(), RefreshToken = GetRefreshToken() }, cancellationToken),
                AvailableImages = await Mediator.Send(new ImagesGetUsersRequest { AccessToken = GetAccessToken(), RefreshToken = GetRefreshToken() }, cancellationToken)
            };
            return PartialView("_Images", model);
        }

        [HttpPost, CommonException]
        public async Task<IActionResult> Images(Guid id, GalleryImageDto[] images, CancellationToken cancellationToken)
        {
            var command = new GalleryImagesSaveCommand
            {
                GalleryId = id,
                Images = images,
                AccessToken = GetAccessToken(),
                RefreshToken = GetRefreshToken()
            };
            await Mediator.Send(command, cancellationToken);
            return Ok();
        }

        [HttpGet, CommonException]
        public async Task<IActionResult> AddImage(Guid id, CancellationToken cancellationToken)
        {
            var model = new AddGalleryImageViewModel
            {
                GalleryId = id,
                AvailableImages = await Mediator.Send(new ImagesGetUsersRequest
                {
                    AccessToken = GetAccessToken(),
                    RefreshToken = GetRefreshToken()
                }, cancellationToken)
            };
            return PartialView("_AddImage", model);
        }

        [HttpPost, CommonException]
        public async Task<IActionResult> AddImage(GalleryImageAddCommand command, CancellationToken cancellationToken)
        {
            command.AccessToken = GetAccessToken();
            command.RefreshToken = GetRefreshToken();
            await Mediator.Send(command, cancellationToken);
            return Ok();
        }

        [HttpPost, CommonException]
        public async Task<IActionResult> RemoveImage(GalleryImageRemoveCommand command, CancellationToken cancellationToken)
        {
            command.AccessToken = GetAccessToken();
            command.RefreshToken = GetRefreshToken();
            await Mediator.Send(command, cancellationToken);
            return Ok();
        }
    }
}
