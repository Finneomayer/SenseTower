using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SC.SenseTower.Accounts.Constants;
using SC.SenseTower.Accounts.Cqrs.Commands;
using SC.SenseTower.Accounts.Cqrs.Requests;
using SC.SenseTower.Accounts.Dto.Wallets;
using SC.SenseTower.Common.Attributes;
using SC.SenseTower.Common.Controllers;

namespace SC.SenseTower.Accounts.Controllers
{
    [Route(ApiConstants.API_ROOT_SEGMENT + "[controller]/[action]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class WalletsController : BaseController
    {
        public WalletsController(
            IMediator mediator,
            ILogger<WalletsController> logger) : base(mediator, logger)
        {
        }

        /// <summary>
        /// Добавление пользовательского кошелька.
        /// </summary>
        /// <param name="command">Данные кошелька.</param>
        /// <param name="cancellationToken">Автоматически генерируемый сервером токен для прерывания выполнения.</param>
        /// <returns>Внутренний идентификатор добавленного кошелька.</returns>
        /// <remarks></remarks>
        /// <response code="200">Идентификатор добавленного кошелька.</response>
        /// <response code="400">Сообщение об ошибке.</response>
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost, CommonException]
        public async Task<ActionResult<string>> Add(AddWalletCommand command, CancellationToken cancellationToken)
        {
            command.OwnerId = GetUserId();
            return Ok(await Mediator.Send(command, cancellationToken));
        }

        /// <summary>
        /// Изменение данных кошелька пользователя.
        /// </summary>
        /// <param name="command">Новые данные кошелька.</param>
        /// <param name="cancellationToken">Автоматически генерируемый сервером токен для прерывания выполнения.</param>
        /// <returns>Признак успешного выполнения операции.</returns>
        /// <remarks></remarks>
        /// <response code="200">True, если данные кошелька были изменены без ошибок.</response>
        /// <response code="400">Сообщение об ошибке.</response>
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost, CommonException]
        public async Task<ActionResult<bool>> Update(UpdateWalletCommand command, CancellationToken cancellationToken)
        {
            command.OwnerId = GetUserId();
            return Ok(await Mediator.Send(command, cancellationToken));
        }

        /// <summary>
        /// Удаление кошелька пользователя.
        /// </summary>
        /// <param name="walletId">Идентификатор кошелька.</param>
        /// <param name="cancellationToken">Автоматически генерируемый сервером токен для прерывания выполнения.</param>
        /// <returns>Признак успешного выполнения операции.</returns>
        /// <remarks></remarks>
        /// <response code="200">True, если кошелёк пользователя удалён.</response>
        /// <response code="400">Сообщение об ошибке.</response>
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost("{walletId}"), CommonException]
        public async Task<ActionResult<bool>> Delete(string walletId, CancellationToken cancellationToken)
        {
            return Ok(await Mediator.Send(new DeleteWalletCommand
            {
                WalletId = walletId,
                OwnerId = GetUserId()
            }, cancellationToken));
        }

        /// <summary>
        /// Получение списка кошельков пользователя.
        /// </summary>
        /// <param name="cancellationToken">Автоматически генерируемый сервером токен для прерывания выполнения.</param>
        /// <returns>Список кошельков пользователя.</returns>
        /// <remarks></remarks>
        /// <response code="200">Возвращает список кошельков пользователя.</response>
        /// <response code="400">Сообщение об ошибке.</response>
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpGet, CommonException]
        public async Task<ActionResult<WalletItemDto[]>> List(CancellationToken cancellationToken)
        {
            return Ok(await Mediator.Send(new UserWalletListRequest { OwnerId = GetUserId() }, cancellationToken));
        }

        /// <summary>
        /// Получение кошелька пользователя по идентификатору.
        /// </summary>
        /// <param name="walletId">Идентификатор кошелька.</param>
        /// <param name="cancellationToken">Автоматически генерируемый сервером токен для прерывания выполнения.</param>
        /// <returns>Информация о кошельке пользователя.</returns>
        /// <remarks></remarks>
        /// <response code="200">Возвращает информацию о кошельке пользователя.</response>
        /// <response code="400">Сообщение об ошибке.</response>
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpGet("{walletId}"), CommonException]
        public async Task<ActionResult<WalletDto>> Get(string walletId, CancellationToken cancellationToken)
        {
            return Ok(await Mediator.Send(new UserWalletRequest
            {
                OwnerId = GetUserId(),
                WalletId = walletId
            }, cancellationToken));
        }

        /// <summary>
        /// Подтверждение кошелька пользователя.
        /// </summary>
        /// <param name="walletId">Идентификатор кошелька.</param>
        /// <param name="cancellationToken">Автоматически генерируемый сервером токен для прерывания выполнения.</param>
        /// <returns>Признак успешного выполнения операции.</returns>
        /// <remarks></remarks>
        /// <response code="200">Возвращает признак успешного выполнения операции.</response>
        /// <response code="400">Сообщение об ошибке.</response>
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost("{walletId}"), CommonException, Authorize(Roles = RoleNames.VR_ADMIN)]
        public async Task<ActionResult<bool>> Confirm(string walletId, CancellationToken cancellationToken)
        {
            return Ok(await Mediator.Send(new ConfirmWalletCommand { WalletId = walletId }, cancellationToken));
        }
    }
}
