using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SC.SenseTower.Common.Attributes;
using SC.SenseTower.Common.Controllers;
using SC.SenseTower.Common.Models;
using SC.SenseTower.Galleries.Constants;
using SC.SenseTower.Galleries.Cqrs.CreateGallery;
using SC.SenseTower.Galleries.Cqrs.DeleteGallery;
using SC.SenseTower.Galleries.Cqrs.Gallery;
using SC.SenseTower.Galleries.Cqrs.GalleryBySpaceId;
using SC.SenseTower.Galleries.Cqrs.GalleryExists;
using SC.SenseTower.Galleries.Cqrs.GalleryList;
using SC.SenseTower.Galleries.Cqrs.UpdateGallery;
using SC.SenseTower.Galleries.Dto.Galleries;

namespace SC.SenseTower.Galleries.Controllers
{
    [Route(ApiConstants.API_VERSION + "[action]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class GalleriesController : BaseController
    {
        public GalleriesController(
            IMediator mediator,
            ILogger<GalleriesController> logger) : base(mediator, logger)
        {
        }

        /// <summary>
        /// Получение списка галерей.
        /// </summary>
        /// <param name="request">Параметры запроса</param>
        /// <param name="cancellationToken">Автоматически генерируемый сервером токен для прерывания выполнения.</param>
        /// <returns>Возвращает массив объектов галерей постранично</returns>
        /// <remarks></remarks>
        /// <response code="200">Признак успешного выполнения операции.</response>
        /// <response code="400">Сообщение об ошибке.</response>
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost, CommonException]
        public async Task<ActionResult<PagedDataDto<GalleryItemDto>>> Get(GalleryListRequest request, CancellationToken cancellationToken)
        {
            request.AccessToken = GetToken();
            return Ok(await Mediator.Send(request, cancellationToken));
        }

        /// <summary>
        /// Получение галереи по ее идентификатору.
        /// </summary>
        /// <param name="galleryId">Идентификатор галереи.</param>
        /// <param name="cancellationToken">Автоматически генерируемый сервером токен для прерывания выполнения.</param>
        /// <returns>Возвращает объект галереи.</returns>
        /// <remarks></remarks>
        /// <response code="200">Признак успешного выполнения операции.</response>
        /// <response code="400">Сообщение об ошибке.</response>
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpGet("{galleryId}"), CommonException]
        public async Task<ActionResult<GalleryDto?>> Get(Guid galleryId, CancellationToken cancellationToken)
        {
            return Ok(await Mediator.Send(new GalleryRequest { Id = galleryId, AccessToken = GetToken() }, cancellationToken));
        }

        /// <summary>
        /// Получение галереи по идентификатор пространства.
        /// </summary>
        /// <param name="spaceId">Идентификатор пространства.</param>
        /// <param name="cancellationToken">Автоматически генерируемый сервером токен для прерывания выполнения.</param>
        /// <returns>Возвращает объект галереи.</returns>
        /// <remarks></remarks>
        /// <response code="200">Признак успешного выполнения операции.</response>
        /// <response code="400">Сообщение об ошибке.</response>
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpGet("{spaceId}"), CommonException]
        public async Task<ActionResult<GalleryDto?>> BySpaceId(Guid spaceId, CancellationToken cancellationToken)
        {
            return Ok(await Mediator.Send(new GalleryBySpaceIdRequest { SpaceId = spaceId }, cancellationToken));
        }

        /// <summary>
        /// Проверка существования галереи.
        /// </summary>
        /// <param name="galleryId">Идентификатор галереи.</param>
        /// <param name="cancellationToken">Автоматически генерируемый сервером токен для прерывания выполнения.</param>
        /// <returns>Возвращает признак существования галереи.</returns>
        /// <remarks></remarks>
        /// <response code="200">Признак успешного выполнения операции.</response>
        /// <response code="400">Сообщение об ошибке.</response>
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpGet("{galleryId}"), CommonException]
        public async Task<ActionResult<bool>> Exists(Guid galleryId, CancellationToken cancellationToken)
        {
            return Ok(await Mediator.Send(new GalleryExistsRequest { Id = galleryId }, cancellationToken));
        }

        /// <summary>
        /// Создание галереи.
        /// </summary>
        /// <param name="command">Параметры создания галереи.</param>
        /// <param name="cancellationToken">Автоматически генерируемый сервером токен для прерывания выполнения.</param>
        /// <returns>Возвращает идентификатор созданной галереи.</returns>
        /// <remarks></remarks>
        /// <response code="200">Признак успешного выполнения операции.</response>
        /// <response code="400">Сообщение об ошибке.</response>
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost, CommonException]
        public async Task<ActionResult<Guid>> Create(CreateGalleryCommand command, CancellationToken cancellationToken)
        {
            command.AccessToken = GetToken();
            command.UserId = GetUserId();
            return Ok(await Mediator.Send(command, cancellationToken));
        }

        /// <summary>
        /// Изменение параметров галереи.
        /// </summary>
        /// <param name="command">Параметры галереи.</param>
        /// <param name="cancellationToken">Автоматически генерируемый сервером токен для прерывания выполнения.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <response code="200">Успешное изменение параметров галереи.</response>
        /// <response code="400">Сообщение об ошибке.</response>
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost, CommonException]
        public async Task<IActionResult> Update(UpdateGalleryCommand command, CancellationToken cancellationToken)
        {
            command.AccessToken = GetToken();
            command.UserId = GetUserId();
            await Mediator.Send(command, cancellationToken);
            return Ok();
        }

        /// <summary>
        /// Удаление галереи.
        /// </summary>
        /// <param name="galleryId">Идентификатор галереи.</param>
        /// <param name="cancellationToken">Автоматически генерируемый сервером токен для прерывания выполнения.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <response code="200">Успешное изменение параметров галереи.</response>
        /// <response code="400">Сообщение об ошибке.</response>
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost("{galleryId}"), CommonException]
        public async Task<IActionResult> Delete(Guid galleryId, CancellationToken cancellationToken)
        {
            await Mediator.Send(new DeleteGalleryCommand { Id = galleryId }, cancellationToken);
            return Ok();
        }

        ///// <summary>
        ///// Изменение параметров галереи из админки.
        ///// </summary>
        ///// <param name="command">Параметры галереи.</param>
        ///// <param name="cancellationToken">Автоматически генерируемый сервером токен для прерывания выполнения.</param>
        ///// <returns></returns>
        ///// <remarks></remarks>
        ///// <response code="200">Успешное изменение параметров галереи.</response>
        ///// <response code="400">Сообщение об ошибке.</response>
        //[Produces("application/json")]
        //[ProducesResponseType(StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status400BadRequest)]
        //[HttpPost, CommonException]
        //public async Task<IActionResult> AdminUpdate(string json, CancellationToken cancellationToken)
        //{
        //    var command = JsonConvert.DeserializeObject<UpdateGalleryCommand>(json);
        //    command.AccessToken = GetToken();
        //    command.UserId = GetUserId();
        //    await Mediator.Send(command, cancellationToken);
        //    return Ok();
        //}

        /// <summary>
        /// Удаление галереи.
        /// </summary>
        /// <param name="galleryId">Идентификатор галереи.</param>
        /// <param name="cancellationToken">Автоматически генерируемый сервером токен для прерывания выполнения.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <response code="200">Успешное изменение параметров галереи.</response>
        /// <response code="400">Сообщение об ошибке.</response>
        //[Produces("application/json")]
        //[ProducesResponseType(StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status400BadRequest)]
        //[HttpPost("{galleryId"), CommonException]
        //public async Task<IActionResult> Delete(Guid galleryId, CancellationToken cancellationToken)
        //{
        //    await Mediator.Send(new DeleteGalleryCommand { Id = galleryId }, cancellationToken);
        //    return Ok();
        //}

        //public async Task<IActionResult> SeedData(CancellationToken cancellationToken)
        //{
        //    await Mediator.Send(new SeedDataCommand(), cancellationToken);
        //    return Ok();
        //}
    }
}
