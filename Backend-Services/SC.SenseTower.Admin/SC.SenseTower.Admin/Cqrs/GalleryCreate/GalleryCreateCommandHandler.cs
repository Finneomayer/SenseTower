using AutoMapper;
using MediatR;
using SC.SenseTower.Admin.Dto.Galleries;
using SC.SenseTower.Admin.Services;
using SC.SenseTower.Common.Cqrs.Handlers;

namespace SC.SenseTower.Admin.Cqrs.GalleryCreate
{
    public class GalleryCreateCommandHandler : BaseHandler, IRequestHandler<GalleryCreateCommand, Guid?>
    {
        private readonly GalleriesService galleriesService;

        public GalleryCreateCommandHandler(
            ILogger<GalleryCreateCommandHandler> logger,
            IMapper mapper,
            GalleriesService galleriesService) : base(logger, mapper)
        {
            this.galleriesService = galleriesService;
        }

        public async Task<Guid?> Handle(GalleryCreateCommand request, CancellationToken cancellationToken)
        {
            var gallery = Mapper.Map<GalleryCreateRequestDto>(request);
            var result = await galleriesService.Create(request.AccessToken, gallery, cancellationToken);
            return result;
        }
    }
}
