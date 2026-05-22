using AutoMapper;
using MediatR;
using SC.SenseTower.Common.Cqrs.Handlers;
using SC.SenseTower.Common.Models;
using SC.SenseTower.Halls.Cqrs.Requests;
using SC.SenseTower.Halls.Services;

namespace SC.SenseTower.Halls.Cqrs.Handlers
{
    public class LookupHallsRequestHandler : BaseHandler, IRequestHandler<LookupHallsRequest, IEnumerable<LookupItemDto<Guid>>>
    {
        private readonly HallsService hallsService;

        public LookupHallsRequestHandler(
            ILogger<LookupHallsRequestHandler> logger,
            IMapper mapper,
            HallsService hallsService) : base(logger, mapper)
        {
            this.hallsService = hallsService;
        }

        public async Task<IEnumerable<LookupItemDto<Guid>>> Handle(LookupHallsRequest request, CancellationToken cancellationToken)
        {
            var result = await hallsService.Lookup(request.HallIds, cancellationToken);
            return result;
        }
    }
}
