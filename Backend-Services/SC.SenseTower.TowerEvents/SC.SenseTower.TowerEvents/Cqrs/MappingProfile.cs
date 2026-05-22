using AutoMapper;
using SC.SenseTower.TowerEvents.Cqrs.TowerEventCreate;
using SC.SenseTower.TowerEvents.Cqrs.TowerEventUpdate;

namespace SC.SenseTower.TowerEvents.Cqrs
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<TowerEventCreateCommand, Data.Models.TowerEvent>();
            CreateMap<TowerEventUpdateCommand, Data.Models.TowerEvent>();
        }
    }
}
