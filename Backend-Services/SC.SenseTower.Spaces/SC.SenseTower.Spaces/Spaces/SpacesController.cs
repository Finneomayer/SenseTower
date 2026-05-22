using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SC.SenseTower.Common.Attributes;
using SC.SenseTower.Common.Controllers;
using SC.SenseTower.Common.Enums;
using SC.SenseTower.Common.Models;
using SC.SenseTower.Spaces.Constants;
using SC.SenseTower.Spaces.Cqrs.Commands;
using SC.SenseTower.Spaces.Cqrs.Requests;
using SC.SenseTower.Spaces.Dto.Spaces;

namespace SC.SenseTower.Spaces.Spaces
{
    [Route(ApiConstants.API_ROOT_SEGMENT + "[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class SpacesController : BaseController
    {
        public SpacesController(IMediator mediator,
            ILogger<SpacesController> logger) : base(mediator, logger)
        {
        }

        /// <summary>
        /// Получение пространства по идентификатору.
        /// </summary>
        /// <param name="spaceId">Идентификатор пространства.</param>
        /// <param name="cancellationToken">Автоматически генерируемый сервером токен для прерывания выполнения.</param>
        /// <returns>Пространство.</returns>
        /// <remarks></remarks>
        /// <response code="200">Признак успешного выполнения операции.</response>
        /// <response code="400">Сообщение об ошибке.</response>
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpGet("{spaceId}"), CommonException]
        public async Task<ActionResult<SpaceDto>> Get(Guid spaceId, CancellationToken cancellationToken)
        {
            return Ok(await Mediator.Send(new SpaceRequest { SpaceId = spaceId }, cancellationToken));
        }

        /// <summary>
        /// Получение всех пространств.
        /// </summary>
        /// <param name="cancellationToken">Автоматически генерируемый сервером токен для прерывания выполнения.</param>
        /// <returns>Пространства.</returns>
        /// <remarks></remarks>
        /// <response code="200">Признак успешного выполнения операции.</response>
        /// <response code="400">Сообщение об ошибке.</response>
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpGet, CommonException]
        public async Task<ActionResult<IEnumerable<SpaceItemDto>>> Get(CancellationToken cancellationToken)
        {
            return Ok(await Mediator.Send(new AllSpacesRequest(), cancellationToken));
        }

        /// <summary>
        /// Создание пространства.
        /// </summary>
        /// <param name="command">Параметры нового пространства.</param>
        /// <param name="cancellationToken">Автоматически генерируемый сервером токен для прерывания выполнения.</param>
        /// <returns>Идентификатор созданного пространства.</returns>
        /// <remarks></remarks>
        /// <response code="200">Признак успешного выполнения операции.</response>
        /// <response code="400">Сообщение об ошибке.</response>
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost(), CommonException]
        public async Task<ActionResult<Guid>> Create(CreateSpaceCommand command, CancellationToken cancellationToken)
        {
            command.AccessToken = GetToken();
            return Ok(await Mediator.Send(command, cancellationToken));
        }

        /// <summary>
        /// Изменение пространства.
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
        [HttpPut(), CommonException]
        public async Task<IActionResult> Update(UpdateSpaceCommand command, CancellationToken cancellationToken)
        {
            command.AccessToken = GetToken();
            await Mediator.Send(command, cancellationToken);
            return Ok();
        }

        /// <summary>
        /// Удаление пространства по идентификатору.
        /// </summary>
        /// <param name="id">Идентификатор пространства.</param>
        /// <param name="cancellationToken">Автоматически генерируемый сервером токен для прерывания выполнения.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <response code="200">Признак успешного выполнения операции.</response>
        /// <response code="400">Сообщение об ошибке.</response>
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpDelete("{id}"), CommonException]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            await Mediator.Send(new DeleteSpaceCommand { Id = id }, cancellationToken);
            return Ok();
        }

        /// <summary>
        /// Получение списка пространств для использования в комбобоксах.
        /// </summary>
        /// <param name="type">Фильтр по типу пространства.</param>
        /// <param name="cancellationToken">Автоматически генерируемый сервером токен для прерывания выполнения.</param>
        /// <returns>Массив пар идентификатор-название.</returns>
        /// <remarks></remarks>
        /// <response code="200">Признак успешного выполнения операции.</response>
        /// <response code="400">Сообщение об ошибке.</response>
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpGet("lookup/{type?}"), CommonException]
        public async Task<ActionResult<IEnumerable<LookupItemDto<Guid>>>> Lookup(SpaceType? type, CancellationToken cancellationToken)
        {
            return Ok(await Mediator.Send(new LookupRequest { SpaceType = type }, cancellationToken));
        }

        [HttpPost("seed"), CommonException, ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> SeedData(CancellationToken cancellationToken)
        {
            _ = await Mediator.Send(new SeedDataCommand { AccessToken = GetToken() }, cancellationToken);
            return Ok();
        }

        /// <summary>
        /// Замена коллекции изображений в пространстве.
        /// </summary>
        /// <param name="command">Новый словарь изображений в пространстве.</param>
        /// <param name="cancellationToken">Автоматически генерируемый сервером токен для прерывания выполнения.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <response code="200">Признак успешного выполнения операции.</response>
        /// <response code="400">Сообщение об ошибке.</response>
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPut("images/replaceall"), CommonException]
        public async Task<IActionResult> ReplaceAll(ReplaceImagesCommand command, CancellationToken cancellationToken)
        {
            command.AccessToken = GetToken();
            command.UserId = GetUserId();
            command.Role = GetRole();
            await Mediator.Send(command, cancellationToken);
            return Ok();
        }

        /// <summary>
        /// Добавление изображения в коллекцию пространства.
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
        [HttpPost("images"), CommonException]
        public async Task<IActionResult> Add(AddImageCommand command, CancellationToken cancellationToken)
        {
            command.AccessToken = GetToken();
            command.UserId = GetUserId();
            await Mediator.Send(command, cancellationToken);
            return Ok();
        }

        /// <summary>
        /// Удаление изображения из коллекции пространства.
        /// </summary>
        /// <param name="command">Идентификаторы пространства и изображения.</param>
        /// <param name="cancellationToken">Автоматически генерируемый сервером токен для прерывания выполнения.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <response code="200">Признак успешного выполнения операции.</response>
        /// <response code="400">Сообщение об ошибке.</response>
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpDelete("images"), CommonException]
        public async Task<IActionResult> Delete(DeleteImageCommand command, CancellationToken cancellationToken)
        {
            command.AccessToken = GetToken();
            await Mediator.Send(command, cancellationToken);
            return Ok();
        }

        /// <summary>
        /// Установка изображения на двери пространства.
        /// </summary>
        /// <param name="command">Идентификаторы пространства и изображения.</param>
        /// <param name="cancellationToken">Автоматически генерируемый сервером токен для прерывания выполнения.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <response code="200">Признак успешного выполнения операции.</response>
        /// <response code="400">Сообщение об ошибке.</response>
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPut("{spaceId}/doorimage/{imageId?}"), CommonException]
        public async Task<IActionResult> DoorImage([FromRoute] SetDoorImageCommand command, CancellationToken cancellationToken)
        {
            command.AccessToken = GetToken();
            await Mediator.Send(command, cancellationToken);
            return Ok();
        }

        /// <summary>
        /// Список пространств, принадлежащих текущему пользователю.
        /// </summary>
        /// <param name="cancellationToken">Автоматически генерируемый сервером токен для прерывания выполнения.</param>
        /// <returns>Массив пространств.</returns>
        /// <remarks></remarks>
        /// <response code="200">Признак успешного выполнения операции.</response>
        /// <response code="400">Сообщение об ошибке.</response>
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpGet("info/owned"), CommonException]
        public async Task<ActionResult<IEnumerable<SpaceDto>>> OwnedSpaces(CancellationToken cancellationToken)
        {
            return Ok(await Mediator.Send(new SpacesByOwnerRequest { UserId = GetUserId() }, cancellationToken));
        }
    }
}
