using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SC.SenseTower.Common.Attributes;
using SC.SenseTower.Common.Controllers;
using SC.SenseTower.Spaces.Constants;
using SC.SenseTower.Spaces.State.CheckIsUserImSpace;
using SC.SenseTower.Spaces.State.GetUsersInSpace;
using SC.SenseTower.Spaces.State.GetUsersInSpaces;
using SC.SenseTower.Spaces.State.RegisterUserInSpace;
using SC.SenseTower.Spaces.State.RegisterUsersInSpace;

namespace SC.SenseTower.Spaces.State
{
    [Route(ApiConstants.API_ROOT_SEGMENT + "[controller]/[action]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class StatesController : BaseController
    {
        public StatesController(IMediator mediator, ILogger<StatesController> logger) : base(mediator, logger) { }

        /// <summary>
        /// Регистрация нескольких пользователей в пространстве.
        /// </summary>
        /// <param name="spaceId">Идентификатор пространства.</param>
        /// <param name="userIds">Массив идентификаторов пользователей.</param>
        /// <param name="cancellationToken">Автоматически генерируемый сервером токен для прерывания выполнения.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <response code="200">Признак успешного выполнения операции.</response>
        /// <response code="400">Сообщение об ошибке.</response>
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost("{spaceId}"), CommonException]
        public async Task<ActionResult> Users(Guid spaceId, [FromBody] Guid[] userIds, CancellationToken cancellationToken)
        {
            return Ok(await Mediator.Send(new RegisterUsersInSpaceCommand { SpaceId = spaceId, UserIds = userIds }, cancellationToken));
        }

        /// <summary>
        /// Проверка нахождения пользователя в пространстве.
        /// </summary>
        /// <param name="spaceId">Идентификатор пространства.</param>
        /// <param name="userId">Идентификатор пользователя.</param>
        /// <param name="cancellationToken">Автоматически генерируемый сервером токен для прерывания выполнения.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <response code="200">Признак успешного выполнения операции.</response>
        /// <response code="400">Сообщение об ошибке.</response>
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpGet("{spaceId}/{userId}"), CommonException]
        public async Task<ActionResult<bool>> Check(Guid spaceId, Guid userId, CancellationToken cancellationToken)
        {
            return Ok(await Mediator.Send(new CheckIsUserInSpaceRequest { SpaceId = spaceId, UserId = userId }, cancellationToken));
        }

        /// <summary>
        /// Получение пользователей, находящихся в пространстве.
        /// </summary>
        /// <param name="getCount">Ограничение на количество возвращаемых пользователей.</param>
        /// <param name="cancellationToken">Автоматически генерируемый сервером токен для прерывания выполнения.</param>
        /// <returns>Список пользователей в помещении.</returns>
        /// <remarks></remarks>
        /// <response code="200">Признак успешного выполнения операции.</response>
        /// <response code="400">Сообщение об ошибке.</response>
        [Produces("application/json")]
        [ProducesResponseType(typeof(IEnumerable<GetUsersInSpacesResponse>), 200)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpGet, CommonException]
        public async Task<ActionResult<GetUsersInSpacesResponse>> Users(int? getCount, CancellationToken cancellationToken)
        {
            return Ok(await Mediator.Send(new GetUsersInSpacesRequest(getCount, GetToken()), cancellationToken));
        }

        /// <summary>
        /// Регистрация пользователя в пространстве.
        /// </summary>
        /// <param name="spaceId">Идентификатор пространства.</param>
        /// <param name="cancellationToken">Автоматически генерируемый сервером токен для прерывания выполнения.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <response code="200">Признак успешного выполнения операции.</response>
        /// <response code="400">Сообщение об ошибке.</response>
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost("{spaceId}"), CommonException]
        public async Task<IActionResult> RegisterUserInSpace(Guid spaceId, CancellationToken cancellationToken)
        {
            await Mediator.Send(new RegisterUserInSpaceCommand { SpaceId = spaceId, UserId = GetUserId() }, cancellationToken);
            return Ok();
        }

        /// <summary>
        /// Получение списка идентификаторов пользователей в пространстве.
        /// </summary>
        /// <param name="spaceId">Идентификатор пространства.</param>
        /// <param name="cancellationToken">Автоматически генерируемый сервером токен для прерывания выполнения.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <response code="200">Признак успешного выполнения операции.</response>
        /// <response code="400">Сообщение об ошибке.</response>
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpGet("{spaceId}"), CommonException]
        public async Task<ActionResult<Guid[]>> UsersInSpace(Guid spaceId, CancellationToken cancellationToken)
        {
            return Ok(await Mediator.Send(new GetUsersInSpaceRequest { SpaceId = spaceId }, cancellationToken));
        }
    }
}
