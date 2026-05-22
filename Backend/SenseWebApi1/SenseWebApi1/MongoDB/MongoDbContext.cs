using Microsoft.Extensions.Options;
using MongoDB.Driver;
using SenseWebApi1.Features.EventFeature;
using SenseWebApi1.Features.TicketFeature;

namespace SenseWebApi1.MongoDB;

public class MongoDbContext:IMongoDbContext
{

        
    private readonly IMongoDatabase _database;
    public MongoDbContext(IOptions<Settings> settings)
    {
        var client = new MongoClient(settings.Value.ConnectionString);
            
        _database = client.GetDatabase(settings.Value.Database);
            
    }

    public IMongoDatabase GetMongoDatabase()
    {
        return _database;
    }

    public async Task EventIniz()
    {
        var mongoCollection = GetMongoDatabase().GetCollection<Event>("Events");
        var events = new List<Event>
        {
            new()
            {
                EventName = "Event-1",
                Description = "Какое то мероприятие 1",
                EventId = Guid.Parse("491d776d-523a-4afa-9b36-a1c46fa126da"),
                Beginning = DateTime.Parse("2023-03-13T17:10:17.5776731+03:00"),
                End = DateTime.Parse("2023-03-13T20:10:17.5776731+03:00"),
                AreaId = Guid.Parse("1f072c8c-b770-4cae-a587-d5e7bb2777ba"),
                ImageId = Guid.Parse("5a448705-8ad7-4456-9847-f3b07e26edf9"),
                IsHavePlaces = true,
                Tickets = new List<Ticket>
                {
                    new()
                    {
                        TicketId = Guid.NewGuid(),
                        Place = 1,
                        OwnerId = Guid.NewGuid()
                    },
                    new()
                    {
                        TicketId = Guid.NewGuid(),
                        Place = 2,
                        OwnerId = Guid.NewGuid()
                    },
                    new()
                    {
                        TicketId = Guid.NewGuid(),
                        Place = 3,
                        OwnerId = Guid.NewGuid()
                    }
                        
                }
            },
            new()
            {
                EventName = "Event-2",
                Description = "Какое то мероприятие 2",
                EventId = Guid.Parse("799b539a-763f-4ce7-b485-850df7ff6cc1"),
                Beginning = DateTime.Parse("2023-03-14T17:10:17.5776731+03:00"),
                End = DateTime.Parse("2023-03-14T20:10:17.5776731+03:00"),
                AreaId = Guid.Parse("558b3257-bb3d-4b40-a2bf-207c77d3149c"),
                ImageId = Guid.Parse("08e8f2cf-7594-4e6d-9df9-77f524ad1e3e"),
                IsHavePlaces = false,
                Tickets = new List<Ticket>
                {
                    new()
                    {
                        TicketId = Guid.NewGuid(),
                        OwnerId = Guid.NewGuid()
                    },
                    new()
                    {
                        TicketId = Guid.NewGuid(),
                        OwnerId = Guid.NewGuid()
                    }
                }
            }
                
        };
        if (await mongoCollection.CountDocumentsAsync(Builders<Event>.Filter.Where(p => true)) == 0)
        {
            await mongoCollection.InsertManyAsync(events);
        }

    }
        
}