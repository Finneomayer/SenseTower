using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SC.SenseTower.Common.Attributes;
using SC.SenseTower.Common.Controllers;
using SC.SenseTower.Common.Models;
using SC.SenseTower.Halls.Constants;
using SC.SenseTower.Halls.Cqrs.Commands;
using SC.SenseTower.Halls.Cqrs.Requests;
using SC.SenseTower.Halls.Dto.Halls;

namespace SC.SenseTower.Halls.Controllers
{
    [Route(ApiConstants.API_ROOT_SEGMENT + "[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class HallsController : BaseController
    {
        public HallsController(
            IMediator mediator,
            ILogger<HallsController> logger) : base(mediator, logger)
        {
        }

        /// <summary>
        /// Получение списка всех доступных текущему пользователю холлов в башне.
        /// </summary>
        /// <param name="cancellationToken">Автоматически генерируемый сервером токен для прерывания выполнения.</param>
        /// <returns>Массив холлов.</returns>
        /// <remarks></remarks>
        /// <response code="200">Возвращает массив холлов.</response>
        /// <response code="400">Сообщение об ошибке.</response>
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpGet, CommonException]
        public async Task<ActionResult<HallListItemDto[]>> List(CancellationToken cancellationToken)
        {
            return Ok(await Mediator.Send(new GetHallsListRequest { AccessToken = GetToken(), UserId = GetUserId() }, cancellationToken));
        }

        /// <summary>
        /// Создание нового холла.
        /// </summary>
        /// <param name="command">Данные нового холла.</param>
        /// <param name="cancellationToken">Автоматически генерируемый сервером токен для прерывания выполнения.</param>
        /// <returns>Идентификатор созданного холла.</returns>
        /// <remarks></remarks>
        /// <response code="200">Возвращает идентификатор созданного холла.</response>
        /// <response code="400">Сообщение об ошибке.</response>
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost, CommonException, Authorize(Roles = RoleNames.VR_ADMIN)]
        public async Task<ActionResult<Guid>> Create([FromBody] CreateHallCommand command, CancellationToken cancellationToken)
        {
            command.AccessToken = GetToken();
            return Ok(await Mediator.Send(command, cancellationToken));
        }

        /// <summary>
        /// Получение холла по его идентификатору.
        /// </summary>
        /// <param name="hallId">Идентификатор холла.</param>
        /// <param name="cancellationToken">Автоматически генерируемый сервером токен для прерывания выполнения.</param>
        /// <returns>Модель холла.</returns>
        /// <remarks></remarks>
        /// <response code="200">Возвращает модель холла.</response>
        /// <response code="400">Сообщение об ошибке.</response>
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpGet("{hallId}"), CommonException]
        public async Task<ActionResult<HallDto?>> Get([FromRoute] Guid hallId, CancellationToken cancellationToken)
        {
            return Ok(await Mediator.Send(new GetHallRequest
            {
                HallId = hallId,
                UserId = GetUserId(),
                AccessToken = GetToken()
            }, cancellationToken));
        }

        /// <summary>
        /// Получение холла по идентификатору входящегов него помещения пользователя.
        /// </summary>
        /// <param name="placeId">Идентификатор помещения пользователя.</param>
        /// <param name="cancellationToken">Автоматически генерируемый сервером токен для прерывания выполнения.</param>
        /// <returns>Модель холла.</returns>
        /// <remarks></remarks>
        /// <response code="200">Возвращает модель холла.</response>
        /// <response code="400">Сообщение об ошибке.</response>
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpGet("byplace/{placeId}"), CommonException]
        public async Task<ActionResult<HallDto?>> GetByPlace([FromRoute] Guid placeId, CancellationToken cancellationToken)
        {
            return Ok(await Mediator.Send(new HallByPlaceIdRequest { PlaceId = placeId }, cancellationToken));
        }

        /// <summary>
        /// Обновление данных холла.
        /// </summary>
        /// <param name="hallId">Идентификатор холла.</param>
        /// <param name="command">Новые данные холла (в текущей реализации только название).</param>
        /// <param name="cancellationToken">Автоматически генерируемый сервером токен для прерывания выполнения.</param>
        /// <returns>True, если операция выполнена успешно.</returns>
        /// <remarks></remarks>
        /// <response code="200">Возвращает признак успешности завершения операции.</response>
        /// <response code="400">Сообщение об ошибке.</response>
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPut("{hallId}"), CommonException, Authorize(Roles = RoleNames.VR_ADMIN)]
        public async Task<ActionResult<bool>> Update([FromRoute] Guid hallId, [FromBody] UpdateHallCommand command, CancellationToken cancellationToken)
        {
            command.Id = hallId;
            command.AccessToken = GetToken();
            return Ok(await Mediator.Send(command, cancellationToken));
        }

        /// <summary>
        /// Удаление холла.
        /// </summary>
        /// <param name="hallId">Идентификатор холла.</param>
        /// <param name="cancellationToken">Автоматически генерируемый сервером токен для прерывания выполнения.</param>
        /// <returns>True, если операция выполнена успешно.</returns>
        /// <remarks></remarks>
        /// <response code="200">Возвращает признак успешности завершения операции.</response>
        /// <response code="400">Сообщение об ошибке.</response>
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpDelete("{hallId}"), CommonException, Authorize(Roles = RoleNames.VR_ADMIN)]
        public async Task<IActionResult> Delete([FromRoute] Guid hallId, CancellationToken cancellationToken)
        {
            return Ok(await Mediator.Send(new DeleteHallCommand
            {
                HallId = hallId,
                UserId = GetUserId()
            }, cancellationToken));
        }

        /// <summary>
        /// Получение списка холлов по их идентификаторам.
        /// </summary>
        /// <param name="hallIds">Массив идентификаторов холлов.</param>
        /// <param name="cancellationToken">Автоматически генерируемый сервером токен для прерывания выполнения.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <response code="200">Возвращает список холлов.</response>
        /// <response code="400">Сообщение об ошибке.</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost("lookup"), CommonException]
        public async Task<ActionResult<IEnumerable<LookupItemDto<Guid>>>> Lookup(Guid[] hallIds, CancellationToken cancellationToken)
        {
            return Ok(await Mediator.Send(new LookupHallsRequest
            {
                HallIds = hallIds
            }, cancellationToken));
        }

        /// <summary>
        /// Добавление пространства в холл.
        /// </summary>
        /// <param name="command">Идентификаторы холла и пространства.</param>
        /// <param name="cancellationToken">Автоматически генерируемый сервером токен для прерывания выполнения.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <response code="200">Успешное завершение операции.</response>
        /// <response code="400">Сообщение об ошибке.</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost("{hallId}/spaces/{spaceId}"), CommonException]
        public async Task<IActionResult> AddSpace([FromRoute] AddSpaceCommand command, CancellationToken cancellationToken)
        {
            command.AccessToken = GetToken();
            await Mediator.Send(command, cancellationToken);
            return Ok();
        }

        /// <summary>
        /// Удаление пространства из холла.
        /// </summary>
        /// <param name="command">Идентификаторы холла и пространства.</param>
        /// <param name="cancellationToken">Автоматически генерируемый сервером токен для прерывания выполнения.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <response code="200">Успешное завершение операции.</response>
        /// <response code="400">Сообщение об ошибке.</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpDelete("{hallId}/spaces/{spaceId}"), CommonException]
        public async Task<IActionResult> RemoveUserPlace([FromRoute] RemoveSpaceCommand command, CancellationToken cancellationToken)
        {
            await Mediator.Send(command, cancellationToken);
            return Ok();
        }

        /// <summary>
        /// Добавление пользовательского помещения в холл.
        /// </summary>
        /// <param name="command">Идентификаторы холла и помещения.</param>
        /// <param name="cancellationToken">Автоматически генерируемый сервером токен для прерывания выполнения.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <response code="200">Успешное завершение операции.</response>
        /// <response code="400">Сообщение об ошибке.</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost("{hallId}/userplaces/{placeId}"), CommonException]
        public async Task<IActionResult> AddUserPlace([FromRoute] AddUserPlaceCommand command, CancellationToken cancellationToken)
        {
            command.AccessToken = GetToken();
            await Mediator.Send(command, cancellationToken);
            return Ok();
        }

        /// <summary>
        /// Удаление пользовательского помещения из холла.
        /// </summary>
        /// <param name="command">Идентификаторы холла и помещения.</param>
        /// <param name="cancellationToken">Автоматически генерируемый сервером токен для прерывания выполнения.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <response code="200">Успешное завершение операции.</response>
        /// <response code="400">Сообщение об ошибке.</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpDelete("{hallId}/userplaces/{placeId}"), CommonException]
        public async Task<IActionResult> RemoveUserPlace(RemoveUserPlaceCommand command, CancellationToken cancellationToken)
        {
            await Mediator.Send(command, cancellationToken);
            return Ok();
        }

        /// <summary>
        /// Добавление публичного помещения в холл.
        /// </summary>
        /// <param name="command">Идентификаторы холла и пространства.</param>
        /// <param name="cancellationToken">Автоматически генерируемый сервером токен для прерывания выполнения.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <response code="200">Успешное завершение операции.</response>
        /// <response code="400">Сообщение об ошибке.</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost("{hallId}/publicplaces/{spaceId}"), CommonException]
        public async Task<IActionResult> AddPublicPlace([FromRoute] AddPublicPlaceCommand command, CancellationToken cancellationToken)
        {
            command.AccessToken = GetToken();
            await Mediator.Send(command, cancellationToken);
            return Ok();
        }

        /// <summary>
        /// Удаление публичного помещения из холла.
        /// </summary>
        /// <param name="command">Идентификаторы холла и пространства.</param>
        /// <param name="cancellationToken">Автоматически генерируемый сервером токен для прерывания выполнения.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <response code="200">Успешное завершение операции.</response>
        /// <response code="400">Сообщение об ошибке.</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpDelete("{hallId}/publicplaces/{spaceId}"), CommonException]
        public async Task<IActionResult> RemovePublicPlace(RemovePublicPlaceCommand command, CancellationToken cancellationToken)
        {
            await Mediator.Send(command, cancellationToken);
            return Ok();
        }

        /// <summary>
        /// Удаление помещений из всех холлов.
        /// </summary>
        /// <param name="placeIds">Массив идентификаторов помещений.</param>
        /// <param name="cancellationToken">Автоматически генерируемый сервером токен для прерывания выполнения.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <response code="200">Успешное завершение операции.</response>
        /// <response code="400">Сообщение об ошибке.</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpDelete("userplaces/clear"), CommonException]
        public async Task<ActionResult<bool>> ClearUserPlaces([FromBody] Guid[] placeIds, CancellationToken cancellationToken)
        {
            await Mediator.Send(new ClearUserPlacesCommand { PlaceIds = placeIds }, cancellationToken);
            return Ok();
        }

        //[HttpPut, CommonException]
        //public async Task<ActionResult<bool>> UpdateUserPlace([FromBody] UserPlaceDto place, CancellationToken cancellationToken)
        //{
        //    //var place = JsonSerializer.Deserialize<UserPlaceDto>(json);
        //    return Ok(await Mediator.Send(new UpdateUserPlaceCommand { Place = place }, cancellationToken));
        //}

        /// <summary>
        /// Создание набора тестовых данных. Одноразовое применение.
        /// </summary>
        /// <param name="cancellationToken">Автоматически генерируемый сервером токен для прерывания выполнения.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <response code="200">Успешное завершение операции.</response>
        /// <response code="400">Сообщение об ошибке.</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost("addtestdata"), CommonException, ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> AddTestData(CancellationToken cancellationToken)
        {
            await Mediator.Send(new AddTestDataCommand(), cancellationToken);
            return Ok();
        }

        [HttpPost("seed"), CommonException, ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> SeedData(CancellationToken cancellationToken)
        {
            _ = await Mediator.Send(new SeedDataCommand { AccessToken = GetToken() }, cancellationToken);
            return Ok();
        }
    }
}
