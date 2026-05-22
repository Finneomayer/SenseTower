using MediatR;
using SC.SenseTower.Admin.Dto;
using SC.SenseTower.Admin.Dto.Places;

namespace SC.SenseTower.Admin.Cqrs.PlaceImagesUpdate
{
    public class PlaceImagesUpdateCommand : ExternalRequestDto, IRequest<Unit>
    {
        public Guid PlaceId { get; set; }

        public PlaceImageSaveDto[] Images { get; set; } = Array.Empty<PlaceImageSaveDto>();
    }
}
