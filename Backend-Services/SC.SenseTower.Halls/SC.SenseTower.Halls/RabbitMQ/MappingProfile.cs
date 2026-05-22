using AutoMapper;
using SC.SenseTower.Halls.Data.Models;
using SC.SenseTower.Halls.Dto.Halls;
using SC.SenseTower.Halls.RabbitMQ.SpaceUpdate;
using SC.SenseTower.Halls.RabbitMQ.UserPlaceUpdate;

namespace SC.SenseTower.Halls.RabbitMQ
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<UserPlaceUpdateCommand, UserPlaceDto>();
            CreateMap<ImageInfoDto, ImageDto>();
            CreateMap<SpaceInfoDto, SpaceDto>()
                .ForMember(d => d.SpaceName, o => o.MapFrom(s => s.Name))
                ;
            CreateMap<SpaceConnectionInfoDto, SpaceConnectionInfo>();
            CreateMap<SpaceUpdateCommand, LocalSpace>()
                .ForMember(d => d.Name, o => o.MapFrom(s => s.SpaceName))
                ;
        }
    }
}
