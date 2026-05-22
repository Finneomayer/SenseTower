using AutoMapper;
using SenseWebApi1.Features.EventFeature.CreateEvent;
using SenseWebApi1.Features.EventFeature.UpdateEvent;

namespace SenseWebApi1.Features.EventFeature;

public class EventProfile:Profile
{
    public EventProfile()
    {
        CreateMap<Event, EventDto>();
        CreateMap<EventDto, Event>();
        CreateMap<EventDto, EventCreateCommand>();
        CreateMap<EventDto, EventUpdateCommand>();
        CreateMap<EventUpdateCommand, Event>();
            
    }
}