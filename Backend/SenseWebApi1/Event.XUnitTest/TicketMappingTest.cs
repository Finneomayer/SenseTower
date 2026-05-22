using AutoMapper;
using SenseWebApi1.Features.TicketFeature;

namespace Event.XUnitTest;

public class TicketMappingTest
{
    [Fact]

    public void TicketToTicketDtoMappingTest()
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<Ticket, TicketDto>();
        });
        
        var mapper = config.CreateMapper();
        var ticketObj = new Ticket
        {
            Id = "6416f87ed4349ac4a67bfe80",
            OwnerId = Guid.Parse("8be7a2a6-4dd8-4390-af8d-c4c78122d5de"),
            Place = 1
        };
        
        
        
        var mappingObj=mapper.Map<Ticket, TicketDto>(ticketObj);
        Assert.IsType<TicketDto>(mappingObj);
        Assert.NotNull(mappingObj.OwnerId);
        Assert.NotNull(mappingObj);
    }
}