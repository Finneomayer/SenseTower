using AutoMapper;
using SC.SenseTower.Cinemas.Data.Models;
using SC.SenseTower.Cinemas.Dto.Cinemas;
using SC.SenseTower.Cinemas.Dto.Spaces;
using SC.SenseTower.Cinemas.Dto.Users;

namespace SC.SenseTower.Cinemas.Dto
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Cinema, CinemaDto>();
            CreateMap<Space, SpaceDto>()
                .ForMember(d => d.SpaceName, o => o.MapFrom(s => s.Name))
                .ReverseMap();
            CreateMap<SpaceConnectionInfo, SpaceConnectionInfoDto>().ReverseMap();
            CreateMap<UserInfo, Users.UserInfoDto>();
            CreateMap<ImageInfo, ImageInfoDto>();
            CreateMap<UserInfoResponseDto, Users.UserInfoDto>()
                .ForMember(d => d.Id, o => o.MapFrom(s => s.UserId))
                ;
            CreateMap<UserInfoResponseDto, UserInfo>()
                .ForMember(d => d.Id, o => o.MapFrom(s => s.UserId))
                ;
            CreateMap<Spaces.UserInfoDto, UserInfo>()
                .ForMember(d => d.Id, o => o.MapFrom(s => s.UserId))
                ;
        }
    }
}
