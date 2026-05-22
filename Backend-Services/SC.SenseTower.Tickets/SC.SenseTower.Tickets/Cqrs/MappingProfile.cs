using AutoMapper;
using SC.SenseTower.Tickets.Cqrs.TicketsCreate;
using SC.SenseTower.Tickets.Data.Models;

namespace SC.SenseTower.Tickets.Cqrs
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<TicketsCreateCommand, Ticket>()
                .ForPath(d => d.TowerEvent.Id, o => o.MapFrom(s => s.EventId))
                ;
            CreateMap<Ticket, Ticket>();
        }
    }
}
