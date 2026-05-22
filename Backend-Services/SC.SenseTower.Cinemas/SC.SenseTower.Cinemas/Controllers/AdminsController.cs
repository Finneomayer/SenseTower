using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SC.SenseTower.Cinemas.Constants;
using SC.SenseTower.Cinemas.Cqrs.AdminAdd;
using SC.SenseTower.Cinemas.Cqrs.AdminDelete;
using SC.SenseTower.Cinemas.Cqrs.AdminList;
using SC.SenseTower.Cinemas.Cqrs.AdminReplaceAll;
using SC.SenseTower.Cinemas.Dto.Users;
using SC.SenseTower.Common.Attributes;
using SC.SenseTower.Common.Controllers;

namespace SC.SenseTower.Cinemas.Controllers
{
    [Route(ApiConstants.API_VERSION + "cinemas/{cinemaId}/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class AdminsController : BaseController
    {
        public AdminsController(
            IMediator mediator,
            ILogger<AdminsController> logger) : base(mediator, logger)
        {
        }

        /// <summary>
        /// Получение списка администраторов кинотеатра по его идентификатору.
        /// </summary>
        /// <param name="cinemaId">Идентификатор кинотеатра.</param>
        /// <param name="cancellationToken">Автоматически генерируемый сервером токен для прерывания выполнения.</param>
        /// <returns>Возвращает массив администраторов.</returns>
        /// <remarks></remarks>
        /// <response code="200">Признак успешного выполнения операции.</response>
        /// <response code="400">Сообщение об ошибке.</response>
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpGet, CommonException]
        public async Task<ActionResult<IEnumerable<UserInfoDto>>> Get(Guid cinemaId, CancellationToken cancellationToken)
        {
            return Ok(await Mediator.Send(new AdminListCommand { CinemaId = cinemaId }, cancellationToken));
        }

        /// <summary>
        /// Добавление администратора кинотеатра.
        /// </summary>
        /// <param name="command">Идентификаторы кинотеатра и администратора.</param>
        /// <param name="cancellationToken">Автоматически генерируемый сервером токен для прерывания выполнения.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <response code="200">Признак успешного выполнения операции.</response>
        /// <response code="400">Сообщение об ошибке.</response>
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost("{userId}"), Authorize(Roles = RoleNames.VR_ADMIN), CommonException]
        public async Task<IActionResult> Add([FromRoute] AdminAddCommand command, CancellationToken cancellationToken)
        {
            command.AccessToken = GetToken();
            await Mediator.Send(command, cancellationToken);
            return Ok();
        }

        /// <summary>
        /// Удаление администратор кинотеатра.
        /// </summary>
        /// <param name="command">Идентификаторы кинотеатра и администратора.</param>
        /// <param name="cancellationToken">Автоматически генерируемый сервером токен для прерывания выполнения.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <response code="200">Признак успешного выполнения операции.</response>
        /// <response code="400">Сообщение об ошибке.</response>
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpDelete("{userId}"), Authorize(Roles = RoleNames.VR_ADMIN), CommonException]
        public async Task<IActionResult> Delete([FromRoute] AdminDeleteCommand command, CancellationToken cancellationToken)
        {
            await Mediator.Send(command, cancellationToken);
            return Ok();
        }

        /// <summary>
        /// Замена списка администраторов кинотеатра новым.
        /// </summary>
        /// <param name="cinemaId">Идентификатор кинотеатра.</param>
        /// <param name="command">Параметры замены.</param>
        /// <param name="cancellationToken">Автоматически генерируемый сервером токен для прерывания выполнения.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        /// <response code="200">Признак успешного выполнения операции.</response>
        /// <response code="400">Сообщение об ошибке.</response>
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPut("[action]"), Authorize(Roles = RoleNames.VR_ADMIN), CommonException]
        public async Task<IActionResult> ReplaceAll([FromRoute] Guid cinemaId, [FromBody] AdminReplaceAllCommand command, CancellationToken cancellationToken)
        {
            command.CinemaId = cinemaId;
            command.AccessToken = GetToken();
            await Mediator.Send(command, cancellationToken);
            return Ok();
        }
    }
}
