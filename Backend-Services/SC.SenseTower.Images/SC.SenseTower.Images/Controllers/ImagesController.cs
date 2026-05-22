using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using SC.SenseTower.Common.Attributes;
using SC.SenseTower.Common.Controllers;
using SC.SenseTower.Images.Constants;
using SC.SenseTower.Images.Cqrs.AddImageFile;
using SC.SenseTower.Images.Cqrs.CopyImage;
using SC.SenseTower.Images.Cqrs.DeleteImage;
using SC.SenseTower.Images.Cqrs.DownloadImage;
using SC.SenseTower.Images.Cqrs.Image;
using SC.SenseTower.Images.Cqrs.ImagesByIds;
using SC.SenseTower.Images.Cqrs.UpdateImage;
using SC.SenseTower.Images.Cqrs.UserImages;
using SC.SenseTower.Images.Dto.Images;

namespace SC.SenseTower.Images.Controllers
{
    [Route(ApiConstants.API_ROOT_SEGMENT + "[controller]/[action]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ImagesController : BaseController
    {
        public ImagesController(
            IMediator mediator,
            ILogger<ImagesController> logger) : base(mediator, logger)
        {
        }

        /// <summary>
        /// Добавление нового изображения пользователю.
        /// </summary>
        /// <param name="command">Параметры изображения.</param>
        /// <param name="cancellationToken">Автоматически генерируемый сервером токен для прерывания выполнения.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <response code="200">Признак успешного выполнения операции.</response>
        /// <response code="400">Сообщение об ошибке.</response>
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost, CommonException]
        public async Task<ActionResult<Guid>> Add(AddImageFileCommand command, CancellationToken cancellationToken)
        {
            command.AccessToken = GetToken();
            return Ok(await Mediator.Send(command, cancellationToken));
        }

        /// <summary>
        /// Чтение списка изображений пользователя.
        /// </summary>
        /// <param name="userId">Идентификатор дополнительного пользователя для админского запроса.</param>
        /// <param name="cancellationToken">Автоматически генерируемый сервером токен для прерывания выполнения.</param>
        /// <returns>Список изображений.</returns>
        /// <remarks></remarks>
        /// <response code="200">Признак успешного выполнения операции.</response>
        /// <response code="400">Сообщение об ошибке.</response>
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpGet(), CommonException]
        public async Task<ActionResult<IEnumerable<ImageListItemDto>>> Get(Guid? userId, CancellationToken cancellationToken)
        {
            var request = new UserImagesRequest
            {
                OwnerId = GetUserId(),
                AccessToken = GetToken(),
                Role = GetRole(),
                UserId = userId,
                RequestUrl = UriHelper.BuildAbsolute(HttpContext.Request.Scheme, HttpContext.Request.Host, HttpContext.Request.PathBase, HttpContext.Request.Path)
            };
            return Ok(await Mediator.Send(request, cancellationToken));
        }

        /// <summary>
        /// Чтение информации об изображении.
        /// </summary>
        /// <param name="id">Идентификатор изображения.</param>
        /// <param name="cancellationToken">Автоматически генерируемый сервером токен для прерывания выполнения.</param>
        /// <returns>Информация об изображении.</returns>
        /// <remarks></remarks>
        /// <response code="200">Признак успешного выполнения операции.</response>
        /// <response code="400">Сообщение об ошибке.</response>
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpGet("{id}"), CommonException]
        public async Task<ActionResult<ImageDto>> Get(Guid id, CancellationToken cancellationToken)
        {
            return Ok(await Mediator.Send(new ImageRequest
            {
                Id = id,
                UserId = GetUserId(),
                AccessToken = GetToken(),
                RequestUrl = UriHelper.BuildAbsolute(HttpContext.Request.Scheme, HttpContext.Request.Host, HttpContext.Request.PathBase, HttpContext.Request.Path)
            }, cancellationToken));
        }

        /// <summary>
        /// Чтение списка изображений по их идентификаторам.
        /// </summary>
        /// <param name="ids">Идентификаторы изображений.</param>
        /// <param name="cancellationToken">Автоматически генерируемый сервером токен для прерывания выполнения.</param>
        /// <returns>Список изображений.</returns>
        /// <remarks></remarks>
        /// <response code="200">Признак успешного выполнения операции.</response>
        /// <response code="400">Сообщение об ошибке.</response>
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost, CommonException]
        public async Task<ActionResult<IEnumerable<ImageInfoDto>>> Get(Guid[] ids, CancellationToken cancellationToken)
        {
            return Ok(await Mediator.Send(new ImagesByIdsRequest
            { 
                ImageIds = ids,
                RequestUrl = UriHelper.BuildAbsolute(HttpContext.Request.Scheme, HttpContext.Request.Host, HttpContext.Request.PathBase, HttpContext.Request.Path)
            }, cancellationToken));
        }

        /// <summary>
        /// Изменение свойств изображения.
        /// </summary>
        /// <param name="command">Обновленные свойства.</param>
        /// <param name="cancellationToken">Автоматически генерируемый сервером токен для прерывания выполнения.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <response code="200">Признак успешного выполнения операции.</response>
        /// <response code="400">Сообщение об ошибке.</response>
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost, CommonException]
        public async Task<IActionResult> Update(UpdateImageCommand command, CancellationToken cancellationToken)
        {
            command.UserId = GetUserId();
            command.AccessToken = GetToken();
            await Mediator.Send(command, cancellationToken);
            return Ok();
        }

        /// <summary>
        /// Удаление изображения.
        /// </summary>
        /// <param name="id">Идентификатор изображения.</param>
        /// <param name="cancellationToken">Автоматически генерируемый сервером токен для прерывания выполнения.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <response code="200">Признак успешного выполнения операции.</response>
        /// <response code="400">Сообщение об ошибке.</response>
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost("{id}"), CommonException]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            await Mediator.Send(new DeleteImageCommand
            {
                Id = id,
                UserId = GetUserId(),
                AccessToken = GetToken()
            }, cancellationToken);
            return Ok();
        }

        /// <summary>
        /// Получение файла изображения.
        /// </summary>
        /// <param name="id">Идентификатор изображения.</param>
        /// <param name="preview">Признак получения превью.</param>
        /// <param name="cancellationToken">Автоматически генерируемый сервером токен для прерывания выполнения.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <response code="200">Признак успешного выполнения операции.</response>
        /// <response code="400">Сообщение об ошибке.</response>
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpGet("{id}"), AllowAnonymous, CommonException]
        public async Task<IActionResult> Download(Guid id, bool preview, CancellationToken cancellationToken)
        {
            var file = await Mediator.Send(new DownloadImageRequest
            {
                Id = id,
                IsPreview = preview,
                UserId = GetUserId()
            }, cancellationToken);
            return File(file.Content, "application/octet-stream", file.Name);
        }

        /// <summary>
        /// Копирование изображения.
        /// </summary>
        /// <param name="id">Идентификатор исходного изображения.</param>
        /// <param name="cancellationToken">Автоматически генерируемый сервером токен для прерывания выполнения.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <response code="200">Признак успешного выполнения операции.</response>
        /// <response code="400">Сообщение об ошибке.</response>
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost("{id}"), CommonException]
        public async Task<ActionResult<ImageInfoDto>> Copy(Guid id, CancellationToken cancellationToken)
        {
            return Ok(await Mediator.Send(new CopyImageCommand
            {
                Id = id,
                UserId = GetUserId()
            }, cancellationToken));
        }
    }
}
