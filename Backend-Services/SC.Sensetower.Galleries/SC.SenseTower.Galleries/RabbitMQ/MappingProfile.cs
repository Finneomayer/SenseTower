using AutoMapper;
using SC.SenseTower.Galleries.Data.Models;
using SC.SenseTower.Galleries.RabbitMQ.SpaceUpdate;

namespace SC.SenseTower.Galleries.RabbitMQ
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
