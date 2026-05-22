using AutoMapper;
using SC.SenseTower.Spaces.Cqrs.Commands;
using SC.SenseTower.Spaces.Data.Models;
using SC.SenseTower.Spaces.Spaces;

namespace SC.SenseTower.Spaces.Cqrs
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<CreateSpaceCommand, Space>()
                .ForMember(d => d.SpaceName, o => o.MapFrom(s => s.SpaceName))
                .ForPath(d => d.SpaceConnectionInfo.Ip, o => o.MapFrom(s => s.Ip))
                .ForPath(d => d.SpaceConnectionInfo.Port, o => o.MapFrom(s => s.Port))
                ;
            CreateMap<UpdateSpaceCommand, Space>()
                .ForMember(d => d.SpaceName, o => o.MapFrom(s => s.SpaceName))
                .ForPath(d => d.SpaceConnectionInfo.Ip, o => o.MapFrom(s => s.Ip))
                .ForPath(d => d.SpaceConnectionInfo.Port, o => o.MapFrom(s => s.Port))
                ;
            CreateMap<TowerSpace, Space>()
                .ForMember(d => d.SpaceName, o => o.MapFrom(s => s.SpaceName))
                .ForMember(d => d.SpaceType, o => o.MapFrom(s => (Common.Enums.SpaceType)(int)s.SpaceType))
                .ForPath(d => d.SpaceConnectionInfo.Ip, o => o.MapFrom(s => s.SpaceConnectionInfo.Ip))
                .ForPath(d => d.SpaceConnectionInfo.Port, o => o.MapFrom(s => s.SpaceConnectionInfo.Port))
                ;
        }
    }
}
