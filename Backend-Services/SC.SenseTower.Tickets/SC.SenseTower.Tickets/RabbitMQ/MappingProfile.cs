using AutoMapper;
using SC.SenseTower.Tickets.Data.Models;
using SC.SenseTower.Tickets.RabbitMQ.TicketsCreate;

namespace SC.SenseTower.Tickets.RabbitMQ
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<TicketsCreateCommand, Ticket>();
            CreateMap<TowerEventDto, TowerEvent>()
                .ForMember(d => d.ImageUrl, o => o.MapFrom(s => s.Image != null ? s.Image.PreviewUrl : null))
                ;
        }
    }
}
