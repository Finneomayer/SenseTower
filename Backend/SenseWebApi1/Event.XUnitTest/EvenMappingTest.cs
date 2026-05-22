using AutoMapper;
using SenseWebApi1.Features.EventFeature;
using SenseWebApi1.Features.TicketFeature;

namespace Event.XUnitTest;

public class EvenMappingToEventDtoTest
{
    [Fact]
    public void EventMappingToEventDtoTest()
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<SenseWebApi1.Features.EventFeature.Event, EventDto>();

        });
        
        var mapper = config.CreateMapper();
        var eventObj = new SenseWebApi1.Features.EventFeature.Event
        {
            Id = "6416f87ed4349ac4a67bfe80",
            EventName = "Event-1",
            Description = "Какое то мероприятие 1",
            EventId = Guid.Parse("491d776d-523a-4afa-9b36-a1c46fa126da"),
            Beginning = DateTime.Parse("2023-03-13T17:10:17.5776731+03:00"),
            End = DateTime.Parse("2023-03-13T20:10:17.5776731+03:00"),
            AreaId = Guid.Parse("ec747803-890d-4ce4-8958-64cfea4e77a7"),
            ImageId = Guid.Parse("bda59f4e-c60b-411b-87e5-61a73125979b"),
            Tickets = new List<Ticket>()
        };
        
        
        
        var mappingObj=mapper.Map<SenseWebApi1.Features.EventFeature.Event, EventDto>(eventObj);
        Assert.IsType<EventDto>(mappingObj);
        Assert.NotNull(mappingObj.Description);
        Assert.NotNull(mappingObj.Tickets);
        Assert.NotNull(mappingObj.EventName);
        
    }
}