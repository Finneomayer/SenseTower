using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SC.SenseTower.Cinemas.Constants;
using SC.SenseTower.Cinemas.Cqrs.AddTestData;
using SC.SenseTower.Cinemas.Cqrs.Cinema;
using SC.SenseTower.Cinemas.Cqrs.CinemaBySpace;
using SC.SenseTower.Cinemas.Cqrs.CinemaCreate;
using SC.SenseTower.Cinemas.Cqrs.CinemaDelete;
using SC.SenseTower.Cinemas.Cqrs.CinemaList;
using SC.SenseTower.Cinemas.Cqrs.CinemaUpdate;
using SC.SenseTower.Cinemas.Dto.Cinemas;
using SC.SenseTower.Common.Attributes;
using SC.SenseTower.Common.Controllers;

namespace SC.SenseTower.Cinemas.Controllers
{
    [Route(ApiConstants.API_VERSION + "[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class CinemasController : BaseController
    {
        public CinemasController(
            IMediator mediator,
            ILogger<CinemasController> logger) : base(mediator, logger)
        {
        }

        /// <summary>
        /// Получение списка всех кинотеатров.
        /// </summary>
        /// <param name="cancellationToken">Автоматически генерируемый сервером токен для прерывания выполнения.</param>
        /// <returns>Возвращает массив объектов кинотеатров.</returns>
        /// <remarks></remarks>
        /// <response code="200">Признак успешного выполнения операции.</response>
        /// <response code="400">Сообщение об ошибке.</response>
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpGet, CommonException]
        public async Task<ActionResult<IEnumerable<CinemaDto>>> Get(CancellationToken cancellationToken)
        {
            var result = await Mediator.Send(new CinemaListRequest(), cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Получение кинотеатра по его идентификатору.
        /// </summary>
        /// <param name="cinemaId">Идентификатор кинотеатра.</param>
        /// <param name="cancellationToken">Автоматически генерируемый сервером токен для прерывания выполнения.</param>
        /// <returns>Возвращает объект кинотеатра.</returns>
        /// <remarks></remarks>
        /// <response code="200">Признак успешного выполнения операции.</response>
        /// <response code="400">Сообщение об ошибке.</response>
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpGet("{cinemaId}"), CommonException]
        public async Task<ActionResult<CinemaDto>> Get(Guid cinemaId, CancellationToken cancellationToken)
        {
            var request = new CinemaRequest
            {
                CinemaId = cinemaId
            };
            var result = await Mediator.Send(request, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Получение кинотеатра по идентификатору пространства.
        /// </summary>
        /// <param name="spaceId">Идентификатор пространства.</param>
        /// <param name="cancellationToken">Автоматически генерируемый сервером токен для прерывания выполнения.</param>
        /// <returns>Возвращает объект кинотеатра.</returns>
        /// <remarks></remarks>
        /// <response code="200">Признак успешного выполнения операции.</response>
        /// <response code="400">Сообщение об ошибке.</response>
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpGet("byspace/{spaceId}"), CommonException]
        public async Task<ActionResult<CinemaDto?>> GetBySpace(Guid spaceId, CancellationToken cancellationToken)
        {
            var request = new CinemaBySpaceRequest
            {
                SpaceId = spaceId
            };
            var result = await Mediator.Send(request, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Создание кинотеатра.
        /// </summary>
        /// <param name="command">Параметры нового кинотеатра.</param>
        /// <param name="cancellationToken">Автоматически генерируемый сервером токен для прерывания выполнения.</param>
        /// <returns>Возвращает идентификатор нового кинотеатра.</returns>
        /// <remarks></remarks>
        /// <response code="200">Признак успешного выполнения операции.</response>
        /// <response code="400">Сообщение об ошибке.</response>
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost, CommonException]
        public async Task<ActionResult<Guid>> Create([FromBody] CinemaCreateCommand command, CancellationToken cancellationToken)
        {
            command.AccessToken = GetToken();
            return Ok(await Mediator.Send(command, cancellationToken));
        }

        /// <summary>
        /// Изменение параметров кинотеатра.
        /// </summary>
        /// <param name="cinemaId">Идентификатор кинотеатра.</param>
        /// <param name="command">Новые параметры кинотеатра.</param>
        /// <param name="cancellationToken">Автоматически генерируемый сервером токен для прерывания выполнения.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <response code="200">Признак успешного выполнения операции.</response>
        /// <response code="400">Сообщение об ошибке.</response>
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPut("{cinemaId}"), CommonException]
        public async Task<IActionResult> Update([FromRoute] Guid cinemaId, [FromBody] CinemaUpdateCommand command, CancellationToken cancellationToken)
        {
            command.Id = cinemaId;
            command.AccessToken = GetToken();
            return Ok(await Mediator.Send(command, cancellationToken));
        }

        /// <summary>
        /// Удаление кинотеатра.
        /// </summary>
        /// <param name="cinemaId">Идентификатор кинотеатра.</param>
        /// <param name="cancellationToken">Автоматически генерируемый сервером токен для прерывания выполнения.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <response code="200">Признак успешного выполнения операции.</response>
        /// <response code="400">Сообщение об ошибке.</response>
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpDelete("{cinemaId}"), CommonException]
        public async Task<IActionResult> Delete(Guid cinemaId, CancellationToken cancellationToken)
        {
            var command = new CinemaDeleteCommand { Id = cinemaId };
            return Ok(await Mediator.Send(command, cancellationToken));
        }

        /// <summary>
        /// Первичное заполнение данными, выполняется одноразово.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost("addtestdata"), CommonException, ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> AddTestData(CancellationToken cancellationToken)
        {
            await Mediator.Send(new AddTestDataCommand(), cancellationToken);
            return Ok();
        }
    }
}
