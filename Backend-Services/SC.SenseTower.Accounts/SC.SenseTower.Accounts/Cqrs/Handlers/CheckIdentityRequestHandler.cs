using AutoMapper;
using MediatR;
using SC.SenseTower.Accounts.Cqrs.Requests;
using SC.SenseTower.Accounts.Dto.Identity;
using SC.SenseTower.Accounts.Services;
using SC.SenseTower.Common.Cqrs.Handlers;

namespace SC.SenseTower.Accounts.Cqrs.Handlers
{
    public class CheckIdentityRequestHandler : BaseHandler, IRequestHandler<CheckIdentityRequest, CheckIdentityResultDto[]>
    {
        private readonly IdentityService identityService;

        public CheckIdentityRequestHandler(
            ILogger<CheckIdentityRequestHandler> logger,
            IMapper mapper,
            IdentityService identityService) : base(logger, mapper)
        {
            this.identityService = identityService;
        }

        public async Task<CheckIdentityResultDto[]> Handle(CheckIdentityRequest request, CancellationToken cancellationToken)
        {
            var result = request.Tokens
                .Select(x => identityService.CheckIdentity(x.UserId, request.AccessToken, x.Token, cancellationToken).GetAwaiter().GetResult())
                .ToArray();
            return await Task.FromResult(result);
        }
    }
}
