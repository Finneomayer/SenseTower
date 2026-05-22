using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SC.SenseTower.Accounts.Constants;
using SC.SenseTower.Accounts.Cqrs.Commands;
using SC.SenseTower.Common.Attributes;
using SC.SenseTower.Common.Controllers;

namespace SC.SenseTower.Accounts.Controllers
{
    [Route(ApiConstants.API_ROOT_SEGMENT + "[controller]/[action]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class InvitesController : BaseController
    {
        public InvitesController(
            IMediator mediator,
            ILogger<InvitesController> logger) : base(mediator, logger)
        {
        }

        /// <summary>
        /// Отзыв выданного приглашения.
        /// </summary>
        /// <param name="inviteId">Идентификатор приглашения.</param>
        /// <param name="reason">Причина отзыва.</param>
        /// <param name="cancellationToken">Автоматически генерируемый сервером токен для прерывания выполнения.</param>
        /// <returns>True, если операция выполнена успешно.</returns>
        /// <remarks></remarks>
        /// <response code="200">Возвращает признак успешного завершения операции.</response>
        /// <response code="400">Сообщение об ошибке.</response>
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost, CommonException]
        public async Task<ActionResult<bool>> Recall(string inviteId, string reason, CancellationToken cancellationToken)
        {
            return Ok(await Mediator.Send(new RecallInviteCommand
            {
                InviteId = inviteId,
                Reason = reason,
                UserId = GetUserId(),
                Role = GetRole()
            }, cancellationToken));
        }

        /// <summary>
        /// Генерация заданного количества инвайтов для админа.
        /// </summary>
        /// <param name="quantity">Число инвайтов для генерации.</param>
        /// <param name="cancellationToken">Автоматически генерируемый сервером токен для прерывания выполнения.</param>
        /// <returns>Список кодов сгенерированных инвайтов.</returns>
        /// <remarks></remarks>
        /// <response code="200">Возвращает массив идентификаторов сгенерированных инвайтов.</response>
        /// <response code="400">Сообщение об ошибке.</response>
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost, CommonException, Authorize(Roles = RoleNames.VR_ADMIN)]
        public async Task<ActionResult<string[]>> Generate(int quantity, CancellationToken cancellationToken)
        {
            return Ok(await Mediator.Send(new GenerateInvitesCommand
            {
                Quantity = quantity,
                Role = GetRole(),
                UserId = GetUserId()
            }, cancellationToken));
        }
    }
}
