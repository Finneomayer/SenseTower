using AutoMapper;
using SC.SenseTower.Common.Models;
using SC.SenseTower.MyPlaces.Data.Models;
using SC.SenseTower.MyPlaces.Dto.Images;
using SC.SenseTower.MyPlaces.Dto.Places;
using SC.SenseTower.MyPlaces.Dto.Spaces;
using SC.SenseTower.MyPlaces.Dto.Users;

namespace SC.SenseTower.MyPlaces.Dto
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Place, UserPlaceDto>()
                .ForMember(d => d.SpaceId, o => o.MapFrom(s => s.Space != null ? s.Space.Id : (Guid?)null))
                .ForMember(d => d.SpaceName, o => o.MapFrom(s => s.Space != null ? s.Space.Name : null))
                .ForMember(d => d.DoorImageUrl, o => o.MapFrom(s => s.DoorImageId != null ? "images/download/" + s.DoorImageId.ToString() : null))
                ;
            //CreateMap<Picture, PlaceImageDto>()
            //    .ForMember(d => d.Id, o => o.MapFrom(s => s.ImageId))
            //    ;
            CreateMap<SpaceConnectionInfoDto, SpaceConnectionInfo>();
            CreateMap<SpaceConnectionDto, SpaceConnectionInfo>().ReverseMap();
            CreateMap<Space, SpaceDto>().ReverseMap();
            CreateMap<SpaceInfoDto, Space>()
                .ForMember(d => d.Name, o => o.MapFrom(s => s.SpaceName))
                ;
            CreateMap<Place, PlaceDto>()
                .ForMember(d => d.Number, o => o.MapFrom(s => s.PlaceNumber))
                .ForMember(d => d.LocalSpace, o => o.MapFrom(s => s.Space))
                .ForPath(d => d.DoorImage.Id, o => o.MapFrom(s => s.DoorImageId))
                .ForMember(d => d.Images, o => o.MapFrom(s => s.Images == null ? null : s.Images.Select(x => new KeyValuePair<int, ImageInfoDto>(x.Location, new ImageInfoDto
                {
                    Id = x.Image.Id == default && x.ImageId != null ? x.ImageId.Value : x.Image.Id,
                    Name = x.Image.Id == default ? string.Empty : x.Image.Name,
                    FileUrl = x.Image.Id == default ? string.Empty : x.Image.FileUrl,
                    PreviewUrl = x.Image.Id == default ? string.Empty : x.Image.PreviewUrl
                }))))
                ;
            CreateMap<Place, PlaceItemDto>()
                .ForMember(d => d.SpaceId, o => o.MapFrom(s => s.Space != null ? s.Space.Id : (Guid?)null))
                .ForMember(d => d.SpaceName, o => o.MapFrom(s => s.Space != null ? s.Space.Name : null))
                .ForMember(d => d.DoorImageUrl, o => o.MapFrom(s => s.DoorImageId != null ? "images/download/" + s.DoorImageId.ToString() : null))
                ;
            CreateMap<Place, LookupItemDto<Guid>>()
                .ForMember(d => d.Name, o => o.MapFrom(s => s.PlaceName))
                ;

            CreateMap<ImageInfo, ImageInfoDto>().ReverseMap();
        }
    }
}
