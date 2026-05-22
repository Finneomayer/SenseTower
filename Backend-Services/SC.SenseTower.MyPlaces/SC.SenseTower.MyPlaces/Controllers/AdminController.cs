using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SC.SenseTower.Common.Attributes;
using SC.SenseTower.Common.Controllers;
using SC.SenseTower.MyPlaces.Constants;
using SC.SenseTower.MyPlaces.Cqrs.Commands;
using SC.SenseTower.MyPlaces.Cqrs.Requests;

namespace SC.SenseTower.MyPlaces.Controllers
{
    [Route(ApiConstants.API_ROOT_SEGMENT + "[controller]/[action]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = RoleNames.VR_ADMIN)]
    public class AdminController : BaseController
    {
        public AdminController(
            IMediator mediator,
            ILogger<AdminController> logger) : base(mediator, logger)
        {
        }

        /// <summary>
        /// Создание помещения.
        /// </summary>
        /// <param name="command">Параметры для создания помещения.</param>
        /// <param name="cancellationToken">Автоматически генерируемый сервером токен для прерывания выполнения.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <response code="200">Возвращает идентификатор помещения.</response>
        /// <response code="400">Сообщение об ошибке.</response>
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost, CommonException]
        public async Task<ActionResult<Guid>> Create(CreatePlaceCommand command, CancellationToken cancellationToken)
        {
            command.AccessToken = GetToken();
            return Ok(await Mediator.Send(command, cancellationToken));
        }

        /// <summary>
        /// Изменение параметров помещения.
        /// </summary>
        /// <param name="command">Новые параметры помещения.</param>
        /// <param name="cancellationToken">Автоматически генерируемый сервером токен для прерывания выполнения.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <response code="200">Успешное завершение операции.</response>
        /// <response code="400">Сообщение об ошибке.</response>
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost, CommonException]
        public async Task<IActionResult> Update(UpdateCommand command, CancellationToken cancellationToken)
        {
            command.AccessToken = GetToken();
            return Ok(await Mediator.Send(command, cancellationToken));
        }

        /// <summary>
        /// Изменение изображения на двери помещения.
        /// </summary>
        /// <param name="command">Параметры изменения.</param>
        /// <param name="cancellationToken">Автоматически генерируемый сервером токен для прерывания выполнения.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <response code="200">Признак успешного выполнения операции.</response>
        /// <response code="400">Сообщение об ошибке.</response>
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost, CommonException]
        public async Task<IActionResult> UpdateImage(UpdateImageCommand command, CancellationToken cancellationToken)
        {
            command.AccessToken = GetToken();
            await Mediator.Send(command, cancellationToken);
            return Ok();
        }

        /// <summary>
        /// Удаление помещения любого пользователя.
        /// </summary>
        /// <param name="id">Идентификатор помещения.</param>
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
            return Ok(await Mediator.Send(new DeletePlaceCommand { PlaceId = id, AccessToken = GetToken() }, cancellationToken));
        }

        /// <summary>
        /// Удаление всех помещений пользователя.
        /// </summary>
        /// <param name="ownerId">Идентификатор пользователя владельца помещений.</param>
        /// <param name="cancellationToken">Автоматически генерируемый сервером токен для прерывания выполнения.</param>
        /// <returns>Признак успешного завершения операции.</returns>
        /// <remarks></remarks>
        /// <response code="200">True, если операция была выполнена успешно.</response>
        /// <response code="400">Сообщение об ошибке.</response>
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost, CommonException]
        public async Task<ActionResult<bool>> DeleteByOwner(Guid? ownerId, CancellationToken cancellationToken)
        {
            return Ok(await Mediator.Send(new DeleteUserPlacesCommand { OwnerId = ownerId, AccessToken = GetToken() }, cancellationToken));
        }

        /// <summary>
        /// Запрос идентификаторов пространств, распределенных по помещениям (админка).
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet, CommonException, ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<IEnumerable<Guid>>> AllocatedSpaceIds(CancellationToken cancellationToken)
        {
            return Ok(await Mediator.Send(new AllocatedSpaceIdsRequest(), cancellationToken));
        }

        [HttpGet, CommonException, ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> AddTestData(CancellationToken cancellationToken)
        {
            await Mediator.Send(new AddTestDataCommand(), cancellationToken);
            return Ok();
        }
    }
}
