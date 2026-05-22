using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using SenseWebApi1.Features.TicketFeature;

namespace SenseWebApi1.Features.EventFeature;

public class Event
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }
    /// <summary>
    /// Id мероприятия
    /// </summary>
    public Guid EventId { get; set; }

    /// <summary>
    /// Момент начала мероприятия
    /// </summary>
    public DateTime Beginning { get; set; }
    /// <summary>
    /// Момент конца мероприятия
    /// </summary>
    public DateTime End { get; set; }
    /// <summary>
    /// Название мероприятия
    /// </summary>
    public string? EventName { get; set; }
    /// <summary>
    /// Описание мероприятия
    /// </summary>
    public string? Description { get; set; }
    /// <summary>
    /// Id картинки
    /// </summary>
    public Guid? ImageId { get; set; }

    public Guid AreaId { get; set; }

    // ReSharper disable once CollectionNeverUpdated.Global
    public List<Ticket>? Tickets { get; set; }
        
    public decimal Cost { get; set; } 
    public bool IsHavePlaces { get; set; }
}