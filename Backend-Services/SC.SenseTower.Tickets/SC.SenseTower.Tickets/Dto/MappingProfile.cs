using AutoMapper;
using SC.SenseTower.Tickets.Data.Models;
using SC.SenseTower.Tickets.Dto.Spaces;
using SC.SenseTower.Tickets.Dto.Tickets;
using SC.SenseTower.Tickets.Dto.TowerEvents;

namespace SC.SenseTower.Tickets.Dto
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Ticket, TicketDto>();
            CreateMap<Ticket, SoldTicketInfoDto>()
                .ForMember(d => d.EventId, o => o.MapFrom(s => s.TowerEvent.Id))
                ;
            CreateMap<TowerEvent, TowerEventDto>();
            CreateMap<Space, SpaceDto>().ReverseMap();
            CreateMap<SpaceConnectionInfo, SpaceConnectionInfoDto>().ReverseMap();
            CreateMap<TowerEventResponseDto, TowerEvent>();
        }
    }
}
