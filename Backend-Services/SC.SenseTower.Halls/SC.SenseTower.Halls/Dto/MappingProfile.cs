using AutoMapper;
using SC.SenseTower.Halls.Data.Models;
using SC.SenseTower.Halls.Dto.Halls;
using SC.SenseTower.Halls.Dto.Places;
using SC.SenseTower.Halls.Dto.Spaces;

namespace SC.SenseTower.Halls.Dto
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Hall, HallListItemDto>();
            CreateMap<Hall, HallDto>()
                .ForMember(d => d.LocalSpace, o => o.MapFrom(s => s.Space))
                ;
            CreateMap<Space, SpaceDto>()
                .ForMember(d => d.SpaceName, o => o.MapFrom(s => s.Name))
                .ReverseMap()
                .ForMember(d => d.Name, o => o.MapFrom(s => s.SpaceName))
                ;
            CreateMap<ImageInfo, ImageDto>().ReverseMap();
            CreateMap<Picture, PictureDto>().ReverseMap();
            CreateMap<Place, UserPlaceDto>()
                .ForMember(d => d.Images, o => o.MapFrom(s => s.Images == null ? null : s.Images.Select(x => new KeyValuePair<int, ImageDto>(x.Location, new ImageDto
                {
                    FileUrl = x.Image.FileUrl,
                    Id = x.Image.Id,
                    Name = x.Image.Name,
                    PreviewUrl = x.Image.PreviewUrl
                }))))
                .ReverseMap()
                .ForMember(d => d.Images, o => o.MapFrom(s => s.Images == null ? null : s.Images.Select(x => new Picture
                {
                    Image = new ImageInfo
                    {
                        FileUrl = x.Value.FileUrl,
                        Id = x.Value.Id,
                        Name = x.Value.Name,
                        PreviewUrl = x.Value.PreviewUrl
                    },
                    Location = x.Key
                })))
                ;
            CreateMap<SpaceInfoDto, Space>()
                .ForMember(d => d.Name, o => o.MapFrom(s => s.SpaceName))
                .ReverseMap()
                .ForMember(d => d.SpaceName, o => o.MapFrom(s => s.Name))
                ;
            CreateMap<SpaceResponseDto, LocalSpace>()
                .ForMember(d => d.Images, o => o.MapFrom(s => s.Images == null ? null : s.Images.Select(x => new Picture
                {
                    Location = x.Key,
                    Image = new ImageInfo
                    {
                        FileUrl = x.Value.FileUrl,
                        Id = x.Value.Id,
                        Name = x.Value.Name,
                        PreviewUrl = x.Value.PreviewUrl
                    }
                })))
                ;
            CreateMap<UserInfoDto, UserInfo>().ReverseMap();
            CreateMap<LocalSpace, Space>();
            CreateMap<LocalSpace, SpaceDto>();
            CreateMap<LocalSpace, LocalSpaceDto>()
                .ForMember(d => d.SpaceName, o => o.MapFrom(s => s.Name))
                .ForMember(d => d.Images, o => o.MapFrom(s => s.Images.Select(x => new KeyValuePair<int, ImageDto>(x.Location, new ImageDto
                {
                    FileUrl = x.Image.FileUrl,
                    Id = x.Image.Id,
                    Name = x.Image.Name,
                    PreviewUrl = x.Image.PreviewUrl
                }))))
                ;
            CreateMap<LocalSpaceDto, Space>()
                .ForMember(d => d.Name, o => o.MapFrom(s => s.SpaceName))
                .ForMember(d => d.Images, o => o.MapFrom(s => s.Images.Select(x => new Picture
                {
                    Location = x.Key,
                    Image = new ImageInfo
                    {
                        FileUrl = x.Value.FileUrl,
                        Id = x.Value.Id,
                        Name = x.Value.Name,
                        PreviewUrl = x.Value.PreviewUrl
                    }
                })))
                .ReverseMap()
                .ForMember(d => d.SpaceName, o => o.MapFrom(s => s.Name))
                .ForMember(d => d.Images, o => o.MapFrom(s => s.Images.Select(x => new KeyValuePair<int, ImageDto>(x.Location, new ImageDto
                {
                    FileUrl = x.Image.FileUrl,
                    Id = x.Image.Id,
                    Name = x.Image.Name,
                    PreviewUrl = x.Image.PreviewUrl
                }))))
                ;
            CreateMap<PlaceInfoDto, Place>()
                .ForMember(d => d.Images, o => o.MapFrom(s => s.Images == null ? null : s.Images.Select(x => new Picture
                {
                    Location = x.Key,
                    Image = new ImageInfo
                    {
                        FileUrl = x.Value.FileUrl,
                        Id = x.Value.Id,
                        Name = x.Value.Name,
                        PreviewUrl = x.Value.PreviewUrl
                    }
                })))
                ;
            CreateMap<PlaceItemDto, Place>()
                .ForMember(d => d.Number, o => o.MapFrom(s => s.PlaceNumber))
                ;
        }
    }
}
