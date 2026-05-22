using AutoMapper;
using MediatR;
using SC.SenseTower.Cinemas.Dto.Users;
using SC.SenseTower.Cinemas.Services;
using SC.SenseTower.Common.Cqrs.Handlers;

namespace SC.SenseTower.Cinemas.Cqrs.AdminList
{
    public class AdminListCommandHandler : BaseHandler, IRequestHandler<AdminListCommand, IEnumerable<UserInfoDto>>
    {
        private readonly CinemasService cinemasService;

        public AdminListCommandHandler(
            ILogger<AdminListCommandHandler> logger,
            IMapper mapper,
            CinemasService cinemasService) : base(logger, mapper)
        {
            this.cinemasService = cinemasService;
        }

        public async Task<IEnumerable<UserInfoDto>> Handle(AdminListCommand request, CancellationToken cancellationToken)
        {
            var cinema = await cinemasService.Get(request.CinemaId, cancellationToken);
            var result = Mapper.Map<UserInfoDto[]>(cinema?.Administrators);
            return result ?? Array.Empty<UserInfoDto>();
        }
    }
}
