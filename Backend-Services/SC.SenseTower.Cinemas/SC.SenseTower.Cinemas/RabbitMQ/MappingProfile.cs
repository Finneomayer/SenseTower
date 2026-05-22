using AutoMapper;
using SC.SenseTower.Cinemas.Data.Models;
using SC.SenseTower.Cinemas.RabbitMQ.SpaceUpdate;

namespace SC.SenseTower.Cinemas.RabbitMQ
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
