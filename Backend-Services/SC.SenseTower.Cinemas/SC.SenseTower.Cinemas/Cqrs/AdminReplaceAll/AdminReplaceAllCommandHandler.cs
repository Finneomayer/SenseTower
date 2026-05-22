using AutoMapper;
using MediatR;
using SC.SenseTower.Cinemas.Data.Models;
using SC.SenseTower.Cinemas.Services;
using SC.SenseTower.Common.Cqrs.Handlers;

namespace SC.SenseTower.Cinemas.Cqrs.AdminReplaceAll
{
    public class AdminReplaceAllCommandHandler : BaseHandler, IRequestHandler<AdminReplaceAllCommand, Unit>
    {
        private readonly CinemasService cinemasService;
        private readonly AccountsService accountsService;

        public AdminReplaceAllCommandHandler(
            ILogger<AdminReplaceAllCommandHandler> logger,
            IMapper mapper,
            CinemasService cinemasService,
            AccountsService accountsService) : base(logger, mapper)
        {
            this.cinemasService = cinemasService;
            this.accountsService = accountsService;
        }

        public async Task<Unit> Handle(AdminReplaceAllCommand request, CancellationToken cancellationToken)
        {
            var userDtos = await accountsService.GetByIds(request.AccessToken, request.UserIds, cancellationToken);
            var admins = Mapper.Map<UserInfo[]>(userDtos);
            await cinemasService.ReplaceAdministrators(request.CinemaId, admins, cancellationToken);
            return Unit.Value;
        }
    }
}
