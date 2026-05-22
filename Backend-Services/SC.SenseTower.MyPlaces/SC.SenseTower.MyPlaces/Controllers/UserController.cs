using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SC.SenseTower.Common.Attributes;
using SC.SenseTower.Common.Controllers;
using SC.SenseTower.Common.Models;
using SC.SenseTower.MyPlaces.Constants;
using SC.SenseTower.MyPlaces.Cqrs.Commands;
using SC.SenseTower.MyPlaces.Cqrs.Requests;
using SC.SenseTower.MyPlaces.Data;
using SC.SenseTower.MyPlaces.Dto.Places;

namespace SC.SenseTower.MyPlaces.Controllers
{
    [Route(ApiConstants.API_ROOT_SEGMENT + "[controller]/[action]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class UserController : BaseController
    {
        private readonly MyPlacesDbContext context;

        public UserController(IMediator mediator, ILogger<UserController> logger, MyPlacesDbContext context) : base(mediator, logger)
        {
            this.context = context;
        }

        /// <summary>
        /// Получение полной информации о помещении, включая изображения.
        /// </summary>
        /// <param name="placeId">Идентификатор помещения.</param>
        /// <param name="cancellationToken">Автоматически генерируемый сервером токен для прерывания выполнения.</param>
        /// <returns>Объект помещения.</returns>
        /// <remarks></remarks>
        /// <response code="200">Признак успешного выполнения операции.</response>
        /// <response code="400">Сообщение об ошибке.</response>
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpGet("{placeId}"), CommonException]
        public async Task<ActionResult<PlaceDto>> Place(Guid placeId, CancellationToken cancellationToken)
        {
            var result = await Mediator.Send(new PlaceRequest { PlaceId = placeId, AccessToken = GetToken() }, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Получение списка помещений текущего пользователя.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <response code="200">Возвращает идентификатор помещения.</response>
        /// <response code="400">Сообщение об ошибке.</response>
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpGet, CommonException]
        public async Task<ActionResult<PlaceDto[]?>> Places(CancellationToken cancellationToken)
        {
            var result = await Mediator.Send(new UserPlacesRequest { UserId = GetUserId(), AccessToken = GetToken() }, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Получение списка всех помещений в башне.
        /// </summary>
        /// <param name="cancellationToken">Автоматически генерируемый сервером токен для прерывания выполнения.</param>
        /// <returns>Массив помещений.</returns>
        /// <remarks></remarks>
        /// <response code="200">Возвращает массив помещений.</response>
        /// <response code="400">Сообщение об ошибке.</response>
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpGet, CommonException]
        public async Task<ActionResult<PlaceDto[]>> AllPlaces(CancellationToken cancellationToken)
        {
            return Ok(await Mediator.Send(new GetAllPlacesRequest { AccessToken = GetToken() }, cancellationToken));
        }

        /// <summary>
        /// Получение списка помещений по их идентификаторам.
        /// </summary>
        /// <param name="placeIds">Список идентификаторов помещений.</param>
        /// <param name="cancellationToken">Автоматически генерируемый сервером токен для прерывания выполнения.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <response code="200">Возвращает массив помещений.</response>
        /// <response code="400">Сообщение об ошибке.</response>
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost, CommonException]
        public async Task<ActionResult<PlaceDto[]>> Places(Guid[] placeIds, CancellationToken cancellationToken)
        {
            var result = await Mediator.Send(new PlacesByIdsRequest { PlaceIds = placeIds, AccessToken = GetToken() }, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Получение помещения по идентификатору пространства.
        /// </summary>
        /// <param name="spaceId">Идентификатор пространства.</param>
        /// <param name="cancellationToken">Автоматически генерируемый сервером токен для прерывания выполнения.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <response code="200">Возвращает помещение.</response>
        /// <response code="400">Сообщение об ошибке.</response>
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpGet("{spaceId}"), CommonException]
        public async Task<ActionResult<PlaceDto?>> PlaceBySpace(Guid spaceId, CancellationToken cancellationToken)
        {
            var result = await Mediator.Send(new PlaceBySpaceRequest { SpaceId = spaceId }, cancellationToken);
            return Ok(result);
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
        /// Создание помещения пользователя.
        /// </summary>
        /// <param name="name">Название помещения.</param>
        /// <param name="cancellationToken">Автоматически генерируемый сервером токен для прерывания выполнения.</param>
        /// <returns>Идентификатор созданного помещения.</returns>
        /// <remarks></remarks>
        /// <response code="200">Признак успешного выполнения операции.</response>
        /// <response code="400">Сообщение об ошибке.</response>
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost, CommonException]
        public async Task<ActionResult<Guid>> Create(string name, CancellationToken cancellationToken)
        {
            return Ok(await Mediator.Send(new CreatePlaceCommand
            {
                OwnerId = GetUserId(),
                PlaceName = name,
                PublicAccessType = Common.Enums.AccessType.Public,
                AccessToken = GetToken()
            }, cancellationToken));
        }

        /// <summary>
        /// Получение списка помещений по их идентификаторам в форме идентификатор - наименование.
        /// </summary>
        /// <param name="placeIds">Массив идентификаторов помещений.</param>
        /// <param name="cancellationToken">Автоматически генерируемый сервером токен для прерывания выполнения.</param>
        /// <returns>Список помещений.</returns>
        /// <remarks></remarks>
        /// <response code="200">Признак успешного выполнения операции.</response>
        /// <response code="400">Сообщение об ошибке.</response>
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost, CommonException]
        public async Task<ActionResult<IEnumerable<LookupItemDto<Guid>>>> Lookup(Guid[] placeIds, CancellationToken cancellationToken)
        {
            return Ok(await Mediator.Send(new LookupPlacesRequest { PlaceIds = placeIds }, cancellationToken));
        }

        /// <summary>
        /// Удаление помещения.
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
        public async Task<ActionResult<bool>> Delete(Guid id, CancellationToken cancellationToken)
        {
            return Ok(await Mediator.Send(new DeleteUserPlaceCommand { PlaceId = id, AccessToken = GetToken() }, cancellationToken));
        }

        /// <summary>
        /// Изменение параметров помещения.
        /// </summary>
        /// <param name="command">Новые параметры помещения.</param>
        /// <param name="cancellationToken">Автоматически генерируемый сервером токен для прерывания выполнения.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <response code="200">Признак успешного выполнения операции.</response>
        /// <response code="400">Сообщение об ошибке.</response>
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost, CommonException]
        public async Task<IActionResult> Update(UpdateUserPlaceCommand command, CancellationToken cancellationToken)
        {
            command.UserId = GetUserId();
            command.AccessToken = GetToken();
            return Ok(await Mediator.Send(command, cancellationToken));
        }

        #region Запросы для админки

        [HttpPost, CommonException, ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<PagedDataDto<PlaceDto>>> PlacesPage([FromBody] AdminPlacesPageRequest request, CancellationToken cancellationToken)
        {
            request.AccessToken = GetToken();
            return Ok(await Mediator.Send(request, cancellationToken));
        }

        [HttpGet("{ownerId}"), CommonException, ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<LookupItemDto<Guid>>> ByOwner(Guid ownerId, CancellationToken cancellationToken)
        {
            var request = new LookupByOwnerRequest { OwnerId = ownerId };
            return Ok(await Mediator.Send(request, cancellationToken));
        }

        #endregion
    }
}
