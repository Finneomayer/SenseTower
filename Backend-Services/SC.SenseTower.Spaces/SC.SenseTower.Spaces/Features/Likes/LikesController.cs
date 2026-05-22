using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SC.SenseTower.Common.Attributes;
using SC.SenseTower.Common.Controllers;
using SC.SenseTower.Spaces.Constants;

namespace SC.SenseTower.Spaces.Features.Likes
{
    [Route(ApiConstants.API_ROOT_SEGMENT + "spaces")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class LikesController : BaseController
    {
        public LikesController(
            IMediator mediator,
            ILogger<LikesController> logger) : base(mediator, logger)
        {
        }

        /// <summary>
        /// выставление или снятие лайка/дизлайка пространству
        /// </summary>
        /// <param name="id">Идентификатор пространства.</param>
        /// <param name="requestBody">Параметры запроса</param>
        /// <param name="cancellationToken">Автоматически генерируемый сервером токен для прерывания выполнения.</param>
        /// <returns>Пространство.</returns>
        /// <remarks></remarks>
        /// <response code="200">Признак успешного выполнения операции.</response>
        /// <response code="400">Сообщение об ошибке.</response>
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost("{id}/like"), CommonException]
        public async Task<IActionResult> Like(Guid id, [FromBody] LikeRequest requestBody, CancellationToken cancellationToken)
        {
            var command = new LikeCommand
            {
                Id = id,
                UserId = GetUserId(),
                Like = requestBody.Like
            };
            _ = await Mediator.Send(command, cancellationToken);

            return Ok();
        }
    }
}
