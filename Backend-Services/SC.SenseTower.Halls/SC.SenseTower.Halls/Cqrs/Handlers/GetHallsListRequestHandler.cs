using AutoMapper;
using MediatR;
using SC.SenseTower.Common.Cqrs.Handlers;
using SC.SenseTower.Halls.Cqrs.Requests;
using SC.SenseTower.Halls.Dto.Halls;
using SC.SenseTower.Halls.Services;

namespace SC.SenseTower.Halls.Cqrs.Handlers
{
    public class GetHallsListRequestHandler : BaseHandler, IRequestHandler<GetHallsListRequest, HallListItemDto[]>
    {
        private readonly HallsService hallsService;

        public GetHallsListRequestHandler(
            ILogger<GetHallsListRequestHandler> logger,
            IMapper mapper,
            HallsService hallsService) : base(logger, mapper)
        {
            this.hallsService = hallsService;
        }

        public async Task<HallListItemDto[]> Handle(GetHallsListRequest request, CancellationToken cancellationToken)
        {
            var data = await hallsService.GetList(request.UserId, cancellationToken);
            var result = Mapper.Map<HallListItemDto[]>(data);
            return result;
        }
    }
}
