using AutoMapper;
using SC.SenseTower.MyPlaces.Data.Models;
using SC.SenseTower.MyPlaces.RabbitMQ.SpaceUpdate;

namespace SC.SenseTower.MyPlaces.RabbitMQ
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<SpaceUpdateCommand, Space>()
                .ForMember(d => d.Name, o => o.MapFrom(s => s.SpaceName))
                ;
        }
    }
}
