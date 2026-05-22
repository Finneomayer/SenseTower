using AutoMapper;
using MediatR;
using SC.SenseTower.Accounts.Cqrs.Commands;
using SC.SenseTower.Accounts.Dto.Identity;
using SC.SenseTower.Accounts.Services;
using SC.SenseTower.Common.Cqrs.Handlers;

namespace SC.SenseTower.Accounts.Cqrs.Handlers
{
    public class ResetPasswordCommandHandler : BaseHandler, IRequestHandler<ResetPasswordCommand, OperationResultDto>
    {
        private readonly IdentityService identityService;
        private readonly AccountsService accountsService;

        public ResetPasswordCommandHandler(
            ILogger<ResetPasswordCommandHandler> logger,
            IMapper mapper,
            IdentityService identityService,
            AccountsService accountsService) : base(logger, mapper)
        {
            this.identityService = identityService;
            this.accountsService = accountsService;
        }

        public async Task<OperationResultDto> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
        {
            var result = new OperationResultDto
            {
                Message = await identityService.SetPassword(request.UserId, request.Password, request.Token, cancellationToken)
            };
            if (string.IsNullOrEmpty(result.Message))
            {
                try
                {
                    await accountsService.ClearPasswordResetInfo(request.UserId.Value, cancellationToken);
                    result.Succeeded = true;
                }
                catch (Exception ex)
                {
                    result.Message = $"Не удалось скорректировать аккаунт: {ex.Message}";
                }
            }
            return result;
        }
    }
}
