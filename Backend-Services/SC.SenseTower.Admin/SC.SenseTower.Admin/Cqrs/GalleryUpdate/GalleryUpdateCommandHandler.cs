using AutoMapper;
using MediatR;
using SC.SenseTower.Admin.Dto.Galleries;
using SC.SenseTower.Admin.Services;
using SC.SenseTower.Common.Cqrs.Handlers;

namespace SC.SenseTower.Admin.Cqrs.GalleryUpdate
{
    public class GalleryUpdateCommandHandler : BaseHandler, IRequestHandler<GalleryUpdateCommand, Unit>
    {
        private readonly GalleriesService galleriesService;

        public GalleryUpdateCommandHandler(
            ILogger<GalleryUpdateCommandHandler> logger,
            IMapper mapper,
            GalleriesService galleriesService) : base(logger, mapper)
        {
            this.galleriesService = galleriesService;
        }

        public async Task<Unit> Handle(GalleryUpdateCommand request, CancellationToken cancellationToken)
        {
            var gallery = Mapper.Map<GalleryUpdateRequestDto>(request);
            await galleriesService.Update(request.AccessToken, gallery, cancellationToken);
            return Unit.Value;
        }
    }
}
