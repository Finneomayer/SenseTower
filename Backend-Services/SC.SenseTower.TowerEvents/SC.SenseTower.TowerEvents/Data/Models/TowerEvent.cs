using MongoDB.Bson.Serialization.Attributes;
using MongoDbGenericRepository.Attributes;
using SC.SenseTower.Common.Enums;
using SC.SenseTower.TowerEvents.Constants;

namespace SC.SenseTower.TowerEvents.Data.Models
{
    [CollectionName(DbConstants.COLLECTION_TOWER_EVENTS)]
    [BsonIgnoreExtraElements]
    public class TowerEvent
    {
        [BsonId]
        public Guid Id { get; set; }

        public DateTimeOffset Date { get; set; }

        public DateTimeOffset UpTo { get; set; }

        public string Title { get; set; } = null!;

        public string Description { get; set; } = null!;

        public ImageInfo? Image { get; set; }

        public Space? Space { get; set; }

        public TowerEventState State { get; set; } = TowerEventState.Planned;

        public Ticket[] SoldTickets { get; set; } = Array.Empty<Ticket>();

        public int TotalTickets { get; set; }
    }
}
