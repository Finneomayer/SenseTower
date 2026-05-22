using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SC.SenseTower.Common.Attributes;
using SC.SenseTower.Common.Controllers;
using SC.SenseTower.Galleries.Constants;
using SC.SenseTower.Galleries.Cqrs.CreateGalleryImage;
using SC.SenseTower.Galleries.Cqrs.DeleteGalleryImage;
using SC.SenseTower.Galleries.Cqrs.GalleryImages;
using SC.SenseTower.Galleries.Cqrs.ReplaceGalleryImages;
using SC.SenseTower.Galleries.Dto.Galleries;

namespace SC.SenseTower.Galleries.Controllers
{
    [Route(ApiConstants.API_VERSION + "[controller]/{galleryId}/[action]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class PicturesController : BaseController
    {
        public PicturesController(
            IMediator mediator,
            ILogger<PicturesController> logger) : base(mediator, logger)
        {
        }

        /// <summary>
        /// Получение списка картин галереи.
        /// </summary>
        /// <param name="galleryId">Идентификатор галереи.</param>
        /// <param name="cancellationToken">Автоматически генерируемый сервером токен для прерывания выполнения.</param>
        /// <returns>Возвращает словарь описателей картин с их расположением в галерее.</returns>
        /// <remarks></remarks>
        /// <response code="200">Признак успешного выполнения операции.</response>
        /// <response code="400">Сообщение об ошибке.</response>
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpGet, CommonException]
        public async Task<ActionResult<Dictionary<int, GalleryImageDto>>> Get(Guid galleryId, CancellationToken cancellationToken)
        {
            return Ok(await Mediator.Send(new GalleryImagesRequest { Id = galleryId, AccessToken = GetToken() }, cancellationToken));
        }

        /// <summary>
        /// Добавление картинки в галерею.
        /// </summary>
        /// <param name="command">Параметры новой картинки.</param>
        /// <param name="cancellationToken">Автоматически генерируемый сервером токен для прерывания выполнения.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <response code="200">Признак успешного выполнения операции.</response>
        /// <response code="400">Сообщение об ошибке.</response>
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost, CommonException]
        public async Task<IActionResult> Create(CreateGalleryImageCommand command, CancellationToken cancellationToken)
        {
            command.UserId = GetUserId();
            command.AccessToken = GetToken();
            await Mediator.Send(command, cancellationToken);
            return Ok();
        }

        /// <summary>
        /// Удаление картинки из галереи.
        /// </summary>
        /// <param name="command">Идентификатор галереи и индекс позиции картинки.</param>
        /// <param name="cancellationToken">Автоматически генерируемый сервером токен для прерывания выполнения.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <response code="200">Признак успешного выполнения операции.</response>
        /// <response code="400">Сообщение об ошибке.</response>
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost("{position}"), CommonException]
        public async Task<IActionResult> Delete(DeleteGalleryImageCommand command, CancellationToken cancellationToken)
        {
            await Mediator.Send(command, cancellationToken);
            return Ok();
        }

        /// <summary>
        /// Замена всего списка изображений галереи.
        /// </summary>
        /// <param name="command">Идентификатор галереи и новый список изображений.</param>
        /// <param name="cancellationToken">>Автоматически генерируемый сервером токен для прерывания выполнения.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <response code="200">Признак успешного выполнения операции.</response>
        /// <response code="400">Сообщение об ошибке.</response>
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost, CommonException]
        public async Task<IActionResult> Replace(ReplaceGalleryImagesCommand command, CancellationToken cancellationToken)
        {
            command.AccessToken = GetToken();
            await Mediator.Send(command, cancellationToken);
            return Ok();
        }
    }
}
