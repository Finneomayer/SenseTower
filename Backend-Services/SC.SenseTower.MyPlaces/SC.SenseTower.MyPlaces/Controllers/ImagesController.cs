using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SC.SenseTower.Common.Attributes;
using SC.SenseTower.Common.Controllers;
using SC.SenseTower.MyPlaces.Constants;
using SC.SenseTower.MyPlaces.Cqrs.Commands;

namespace SC.SenseTower.MyPlaces.Controllers
{
    [Route(ApiConstants.API_ROOT_SEGMENT + "[controller]/[action]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ImagesController : BaseController
    {
        public ImagesController(IMediator mediator, ILogger<ImagesController> logger) : base(mediator, logger)
        {
        }

        /// <summary>
        /// Замена коллекции изображений в помещении.
        /// </summary>
        /// <param name="command">Новый словарь изображений в помещении</param>
        /// <param name="cancellationToken">Автоматически генерируемый сервером токен для прерывания выполнения.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <response code="200">Признак успешного выполнения операции.</response>
        /// <response code="400">Сообщение об ошибке.</response>
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost, CommonException]
        public async Task<IActionResult> ReplaceAll(ReplaceImagesCommand command, CancellationToken cancellationToken)
        {
            command.AccessToken = GetToken();
            command.UserId = GetUserId();
            command.Role = GetRole();
            await Mediator.Send(command, cancellationToken);
            return Ok();
        }

        /// <summary>
        /// Добавление изображения в коллекцию помещения.
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
        public async Task<IActionResult> Add(AddImageCommand command, CancellationToken cancellationToken)
        {
            command.AccessToken = GetToken();
            command.UserId = GetUserId();
            await Mediator.Send(command, cancellationToken);
            return Ok();
        }

        /// <summary>
        /// Удаление изображения из коллекции помещения.
        /// </summary>
        /// <param name="command">Идентификаторы помещения и изображения.</param>
        /// <param name="cancellationToken">Автоматически генерируемый сервером токен для прерывания выполнения.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <response code="200">Признак успешного выполнения операции.</response>
        /// <response code="400">Сообщение об ошибке.</response>
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost, CommonException]
        public async Task<IActionResult> Delete(DeleteImageCommand command, CancellationToken cancellationToken)
        {
            command.AccessToken = GetToken();
            await Mediator.Send(command, cancellationToken);
            return Ok();
        }
    }
}
