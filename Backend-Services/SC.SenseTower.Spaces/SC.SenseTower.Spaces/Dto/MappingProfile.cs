using AutoMapper;
using SC.SenseTower.Common.Models;
using SC.SenseTower.Spaces.Data.Models;
using SC.SenseTower.Spaces.Dto.Accounts;
using SC.SenseTower.Spaces.Dto.Places;
using SC.SenseTower.Spaces.Dto.Spaces;

namespace SC.SenseTower.Spaces.Dto
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<PlaceInfoResponseDto, PlaceInfoDto>();

            CreateMap<Space, SpaceItemDto>()
                .ForMember(d => d.Images, o => o.MapFrom(s => s.Images.Select(x => new KeyValuePair<int, ImageInfoDto>(x.Location, new ImageInfoDto
                {
                    FileUrl = x.Image.FileUrl,
                    Id = x.Image.Id,
                    Name = x.Image.Name,
                    PreviewUrl = x.Image.PreviewUrl
                }))))
                ;
            CreateMap<Space, SpaceDto>()
                .ForMember(d => d.Images, o => o.MapFrom(s => s.Images.Select(x => new KeyValuePair<int, ImageInfoDto>(x.Location, new ImageInfoDto
                {
                    FileUrl = x.Image.FileUrl,
                    Id = x.Image.Id,
                    Name = x.Image.Name,
                    PreviewUrl = x.Image.PreviewUrl
                }))))
                ;
            CreateMap<Space, LookupItemDto<Guid>>()
                .ForMember(d => d.Name, o => o.MapFrom(s => s.SpaceName))
                ;
            CreateMap<SpaceConnectionInfo, SpaceConnectionInfoDto>().ReverseMap();
            CreateMap<ImageInfo, ImageInfoDto>().ReverseMap();
            CreateMap<UserInfo, UserInfoDto>().ReverseMap();
            CreateMap<PlaceResponseDto, Space>()
                .ForMember(d => d.Id, o => o.Ignore())
                .ForMember(d => d.Images, o => o.MapFrom(s => s.Images == null ? Array.Empty<Picture>() : s.Images.Select(x => new Picture { Image = new ImageInfo
                {
                    FileUrl = x.Value.FileUrl,
                    Id = x.Value.Id,
                    Name = x.Value.Name,
                    PreviewUrl= x.Value.PreviewUrl
                }, Location = x.Key }).ToArray()))
                ;
        }
    }
}
