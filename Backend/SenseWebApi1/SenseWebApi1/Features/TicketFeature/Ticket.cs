using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
// ReSharper disable PropertyCanBeMadeInitOnly.Global

namespace SenseWebApi1.Features.TicketFeature;

public class Ticket
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    public string? Id { get; set; }
        
    public Guid TicketId { get; set; }

    public Guid? OwnerId { get; set; }

    public int? Place { get; set; }

        
}