using AutoMapper;
using MediatR;
using SC.SenseTower.Common.Cqrs.Handlers;
using SC.SenseTower.Halls.Cqrs.Requests;
using SC.SenseTower.Halls.Dto.Halls;
using SC.SenseTower.Halls.Services;

namespace SC.SenseTower.Halls.Cqrs.Handlers
{
    public class HallByPlaceIdRequestHandler : BaseHandler, IRequestHandler<HallByPlaceIdRequest, HallDto?>
    {
        private readonly HallsService hallsService;

        public HallByPlaceIdRequestHandler(
            ILogger<HallByPlaceIdRequestHandler> logger,
            IMapper mapper,
            HallsService hallsService) : base(logger, mapper)
        {
            this.hallsService = hallsService;
        }

        public async Task<HallDto?> Handle(HallByPlaceIdRequest request, CancellationToken cancellationToken)
        {
            var data = await hallsService.GetByPlaceId(request.PlaceId, cancellationToken);
            if (data == null)
                return null;

            var result = Mapper.Map<HallDto>(data);
            return result;
        }
    }
}
