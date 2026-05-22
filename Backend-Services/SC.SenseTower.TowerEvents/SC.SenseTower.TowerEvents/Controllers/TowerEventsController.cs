using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SC.SenseTower.Common.Attributes;
using SC.SenseTower.Common.Controllers;
using SC.SenseTower.TowerEvents.Constants;
using SC.SenseTower.TowerEvents.Cqrs.TowerEventList;
using SC.SenseTower.TowerEvents.Cqrs.TowerEvent;
using SC.SenseTower.TowerEvents.Cqrs.TowerEventCreate;
using SC.SenseTower.TowerEvents.Dto.TowerEvents;
using SC.SenseTower.Common.Enums;
using SC.SenseTower.TowerEvents.Cqrs.TowerEventStateUpdate;
using SC.SenseTower.TowerEvents.Cqrs.TowerEventUpdate;
using SC.SenseTower.TowerEvents.Cqrs.TowerEventDelete;
using SC.SenseTower.TowerEvents.Cqrs.TowerEventTopList;
using SC.SenseTower.TowerEvents.Cqrs.AdminTowerEventList;
using SC.SenseTower.TowerEvents.Cqrs.TowerEventExists;

namespace SC.SenseTower.TowerEvents.Controllers
{
    [Route(ApiConstants.API_VERSION + "[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class TowerEventsController : BaseController
    {
        public TowerEventsController(
            IMediator mediator,
            ILogger<TowerEventsController> logger) : base(mediator, logger)
        {
        }

        /// <summary>
        /// Получение списка планируемых событий.
        /// </summary>
        /// <param name="getCount">Максимльное число возвращаемых событий.</param>
        /// <param name="cancellationToken">Автоматически генерируемый сервером токен для прерывания выполнения.</param>
        /// <returns>Список всех планируемых событий</returns>
        /// <remarks></remarks>
        /// <response code="200">Признак успешного выполнения операции.</response>
        /// <response code="400">Сообщение об ошибке.</response>
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpGet, CommonException]
        public async Task<ActionResult<TowerEventListItemDto[]>> Get(int? getCount, CancellationToken cancellationToken)
        {
            return Ok(await Mediator.Send(new TowerEventTopListRequest { Limit = getCount }, cancellationToken));
        }

        /// <summary>
        /// Получение списка событий по фильтру.
        /// </summary>
        /// <param name="request">Параметры запроса.</param>
        /// <param name="cancellationToken">Автоматически генерируемый сервером токен для прерывания выполнения.</param>
        /// <returns>Список событий по фильтру.</returns>
        /// <remarks></remarks>
        /// <response code="200">Признак успешного выполнения операции.</response>
        /// <response code="400">Сообщение об ошибке.</response>
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost("list"), CommonException]
        public async Task<ActionResult<TowerEventListItemDto[]>> List([FromBody] TowerEventListRequest request, CancellationToken cancellationToken)
        {
            return Ok(await Mediator.Send(request, cancellationToken));
        }

        [HttpPost("admin/list"), CommonException, ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<IEnumerable<TowerEventListItemDto>>> AdminList([FromBody] AdminTowerEventListRequest request, CancellationToken cancellationToken)
        {
            return Ok(await Mediator.Send(request, cancellationToken));
        }

        [HttpGet("exists/{eventId}"), CommonException, ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<bool>> Exists(Guid eventId, CancellationToken cancellationToken)
        {
            return Ok(await Mediator.Send(new TowerEventExistsRequest { EventId = eventId }, cancellationToken));
        }

        /// <summary>
        /// Получение события по его идентификатору.
        /// </summary>
        /// <param name="eventId">Идентификатор события.</param>
        /// <param name="cancellationToken">Автоматически генерируемый сервером токен для прерывания выполнения.</param>
        /// <returns>Найденное событие или null.</returns>
        /// <remarks></remarks>
        /// <response code="200">Признак успешного выполнения операции.</response>
        /// <response code="400">Сообщение об ошибке.</response>
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpGet("{eventId}"), CommonException]
        public async Task<ActionResult<TowerEventDto?>> Get(Guid eventId, CancellationToken cancellationToken)
        {
            return Ok(await Mediator.Send(new TowerEventRequest { EventId = eventId }, cancellationToken));
        }

        /// <summary>
        /// Создание события.
        /// </summary>
        /// <param name="command">Параметры создания события.</param>
        /// <param name="cancellationToken">Автоматически генерируемый сервером токен для прерывания выполнения.</param>
        /// <returns>Идентификатор нового события.</returns>
        /// <remarks></remarks>
        /// <response code="200">Признак успешного выполнения операции.</response>
        /// <response code="400">Сообщение об ошибке.</response>
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost, CommonException]
        public async Task<ActionResult<Guid>> Create([FromBody] TowerEventCreateCommand command, CancellationToken cancellationToken)
        {
            command.AccessToken = GetToken();
            return Ok(await Mediator.Send(command, cancellationToken));
        }

        /// <summary>
        /// Изменение состояния события.
        /// </summary>
        /// <param name="eventId">Идентификатор события.</param>
        /// <param name="state">Новое состояние.</param>
        /// <param name="cancellationToken">Автоматически генерируемый сервером токен для прерывания выполнения.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <response code="200">Признак успешного выполнения операции.</response>
        /// <response code="400">Сообщение об ошибке.</response>
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPut("{eventId}/state/{state}"), CommonException]
        public async Task<IActionResult> UpdateState(Guid eventId, TowerEventState state, CancellationToken cancellationToken)
        {
            var command = new TowerEventStateUpdateCommand
            {
                EventId = eventId,
                State = state
            };
            await Mediator.Send(command, cancellationToken);
            return Ok();
        }

        /// <summary>
        /// Изменение параметров события.
        /// </summary>
        /// <param name="command">Параметры события.</param>
        /// <param name="cancellationToken">Автоматически генерируемый сервером токен для прерывания выполнения.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <response code="200">Признак успешного выполнения операции.</response>
        /// <response code="400">Сообщение об ошибке.</response>
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPut, CommonException]
        public async Task<IActionResult> Update([FromBody] TowerEventUpdateCommand command,  CancellationToken cancellationToken)
        {
            command.AccessToken = GetToken();
            await Mediator.Send(command, cancellationToken);
            return Ok();
        }

        /// <summary>
        /// Удаление события.
        /// </summary>
        /// <param name="eventId">Идентификатор удаляемого события.</param>
        /// <param name="cancellationToken">Автоматически генерируемый сервером токен для прерывания выполнения.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <response code="200">Признак успешного выполнения операции.</response>
        /// <response code="400">Сообщение об ошибке.</response>
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpDelete("{eventId}"), CommonException]
        public async Task<IActionResult> Delete(Guid eventId, CancellationToken cancellationToken)
        {
            await Mediator.Send(new TowerEventDeleteCommand { EventId = eventId }, cancellationToken);
            return Ok();
        }
    }
}
