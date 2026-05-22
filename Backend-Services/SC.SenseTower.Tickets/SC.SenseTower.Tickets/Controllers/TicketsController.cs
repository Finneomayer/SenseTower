using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SC.SenseTower.Common.Attributes;
using SC.SenseTower.Common.Controllers;
using SC.SenseTower.Tickets.Constants;
using SC.SenseTower.Tickets.Cqrs.BuyTicket;
using SC.SenseTower.Tickets.Cqrs.EventTicketList;
using SC.SenseTower.Tickets.Cqrs.TicketsCreate;
using SC.SenseTower.Tickets.Cqrs.TowerEventSoldTicket;
using SC.SenseTower.Tickets.Dto.Tickets;

namespace SC.SenseTower.Tickets.Controllers
{
    [Route(ApiConstants.API_VERSION + "[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class TicketsController : BaseController
    {
        public TicketsController(IMediator mediator, ILogger<TicketsController> logger) : base(mediator, logger)
        {
        }

        /// <summary>
        /// Получение списка всех билетов.
        /// </summary>
        /// <param name="eventId">Идентификатор мероприятия.</param>
        /// <param name="cancellationToken">Автоматически генерируемый сервером токен для прерывания выполнения.</param>
        /// <returns>Возвращает массив билетов.</returns>
        /// <remarks></remarks>
        /// <response code="200">Признак успешного выполнения операции.</response>
        /// <response code="400">Сообщение об ошибке.</response>
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpGet, CommonException]
        public async Task<ActionResult<IEnumerable<TicketDto>>> Get([FromQuery] Guid eventId, CancellationToken cancellationToken)
        {
            var result = await Mediator.Send(new EventTicketListRequest { EventId = eventId }, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Создание билетов.
        /// </summary>
        /// <param name="command">Параметры создания билетов.</param>
        /// <param name="cancellationToken">Автоматически генерируемый сервером токен для прерывания выполнения.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <response code="200">Признак успешного выполнения операции.</response>
        /// <response code="400">Сообщение об ошибке.</response>
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost, CommonException]
        public async Task<IActionResult> Create([FromBody] TicketsCreateCommand command, CancellationToken cancellationToken)
        {
            command.AccessToken = GetToken();
            await Mediator.Send(command, cancellationToken);
            return Ok();
        }

        /// <summary>
        /// Покупка текущим пользователем билета на событие.
        /// </summary>
        /// <param name="eventId">Идентификатор события.</param>
        /// <param name="cancellationToken">Автоматически генерируемый сервером токен для прерывания выполнения.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <response code="200">Признак успешного выполнения операции.</response>
        /// <response code="400">Сообщение об ошибке.</response>
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost("buy/{eventId}"), CommonException]
        public async Task<IActionResult> Buy(Guid eventId, CancellationToken cancellationToken)
        {
            await Mediator.Send(new BuyTicketCommand { EventId = eventId, UserId = GetUserId() }, cancellationToken);
            return Ok();
        }

        /// <summary>
        /// Получение информации о проданных билетах.
        /// </summary>
        /// <param name="eventId">Идентификатор события.</param>
        /// <param name="cancellationToken">Автоматически генерируемый сервером токен для прерывания выполнения.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <response code="200">Признак успешного выполнения операции.</response>
        /// <response code="400">Сообщение об ошибке.</response>
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpGet("sold/{eventId}"), CommonException]
        public async Task<ActionResult<IEnumerable<SoldTicketInfoDto>>> Sold(Guid eventId, CancellationToken cancellationToken)
        {
            return Ok(await Mediator.Send(new TowerEventSoldTicketRequest { EventId = eventId, AccessToken = GetToken() }, cancellationToken));
        }
    }
}
