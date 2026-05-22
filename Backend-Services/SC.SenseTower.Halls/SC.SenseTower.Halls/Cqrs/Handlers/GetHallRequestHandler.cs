using AutoMapper;
using MediatR;
using SC.SenseTower.Common.Cqrs.Handlers;
using SC.SenseTower.Common.Exceptions;
using SC.SenseTower.Common.Extensions;
using SC.SenseTower.Halls.Cqrs.Requests;
using SC.SenseTower.Halls.Dto.Halls;
using SC.SenseTower.Halls.Services;

namespace SC.SenseTower.Halls.Cqrs.Handlers
{
    public class GetHallRequestHandler : BaseHandler, IRequestHandler<GetHallRequest, HallDto?>
    {
        private readonly HallsService hallsService;

        public GetHallRequestHandler(
            ILogger<GetHallRequestHandler> logger,
            IMapper mapper,
            HallsService hallsService) : base(logger, mapper)
        {
            this.hallsService = hallsService;
        }

        public async Task<HallDto?> Handle(GetHallRequest request, CancellationToken cancellationToken)
        {
            var data = await hallsService.Get(request.HallId, cancellationToken);
            if (data == null)
                throw new ScException("Холл не найден.");

            var result = Mapper.Map<HallDto>(data);
            return result;
        }
    }
}
