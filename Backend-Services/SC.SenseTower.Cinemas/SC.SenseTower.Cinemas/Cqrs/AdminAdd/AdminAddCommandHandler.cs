using AutoMapper;
using MediatR;
using SC.SenseTower.Cinemas.Data.Models;
using SC.SenseTower.Cinemas.Services;
using SC.SenseTower.Common.Cqrs.Handlers;

namespace SC.SenseTower.Cinemas.Cqrs.AdminAdd
{
    public class AdminAddCommandHandler : BaseHandler, IRequestHandler<AdminAddCommand, Unit>
    {
        private readonly CinemasService cinemasService;
        private readonly AccountsService accountsService;

        public AdminAddCommandHandler(
            ILogger<AdminAddCommandHandler> logger,
            IMapper mapper,
            CinemasService cinemasService,
            AccountsService accountsService) : base(logger, mapper)
        {
            this.cinemasService = cinemasService;
            this.accountsService = accountsService;
        }

        public async Task<Unit> Handle(AdminAddCommand request, CancellationToken cancellationToken)
        {
            var adminResponse = await accountsService.GetUserInfo(request.AccessToken, request.UserId, cancellationToken);
            var admin = Mapper.Map<UserInfo>(adminResponse);
            await cinemasService.AddAdministrator(request.CinemaId, admin, cancellationToken);
            return Unit.Value;
        }
    }
}
