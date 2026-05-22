using AutoMapper;
using MediatR;
using SC.SenseTower.Admin.Services;
using SC.SenseTower.Common.Cqrs.Handlers;

namespace SC.SenseTower.Admin.Cqrs.FixAccounts
{
    public class FixAccountsCommandHandler : BaseHandler, IRequestHandler<FixAccountsCommand, Unit>
    {
        private readonly IdentityService identityService;
        private readonly AccountsService accountsService;

        public FixAccountsCommandHandler(
            ILogger<FixAccountsCommandHandler> logger,
            IMapper mapper,
            IdentityService identityService,
            AccountsService accountsService) : base(logger, mapper)
        {
            this.identityService = identityService;
            this.accountsService = accountsService;
        }

        public async Task<Unit> Handle(FixAccountsCommand request, CancellationToken cancellationToken)
        {
            var users = await identityService.GetUserLookups(Array.Empty<Guid?>(), cancellationToken);
            var accounts = await accountsService.GetAll(cancellationToken);
            var notExistedIds = users
                .Where(r => !accounts.Any(a => a.Id == r.Id))
                .Select(r => r.Id)
                .ToArray();
            foreach (var userId in notExistedIds)
            {
                await accountsService.Create(userId, cancellationToken);
            }
            return Unit.Value;
        }
    }
}
