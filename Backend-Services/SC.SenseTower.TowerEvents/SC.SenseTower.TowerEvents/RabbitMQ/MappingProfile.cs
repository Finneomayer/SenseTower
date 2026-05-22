using AutoMapper;
using SC.SenseTower.TowerEvents.Data.Models;
using SC.SenseTower.TowerEvents.RabbitMQ.SpaceUpdate;

namespace SC.SenseTower.TowerEvents.RabbitMQ
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<SpaceUpdateCommand, Space>();
        }
    }
}
