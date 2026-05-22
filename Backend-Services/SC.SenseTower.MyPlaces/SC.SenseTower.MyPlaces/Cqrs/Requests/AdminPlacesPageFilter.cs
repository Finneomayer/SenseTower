using MongoDB.Driver;
using SC.SenseTower.Common.Data;
using SC.SenseTower.Common.Enums;
using SC.SenseTower.Common.Services;
using SC.SenseTower.MyPlaces.Data.Models;

namespace SC.SenseTower.MyPlaces.Cqrs.Requests
{
    public class AdminPlacesPageFilter : AbstractFilter<Place>
    {
        public string? PlaceName { get; set; }

        public Guid? OwnerId { get; set; }

        public Guid? SpaceId { get; set; }

        public AccessType? PublicAccessType { get; set; }

        public override async Task<FilterDefinition<Place>> Filter(BaseDbService? service, CancellationToken cancellationToken)
        {
            var filter = Builders<Place>.Filter.Empty;
            if (!string.IsNullOrWhiteSpace(PlaceName))
                filter = Builders<Place>.Filter.And(filter, Builders<Place>.Filter.Where(x => x.PlaceName.StartsWith(PlaceName, StringComparison.InvariantCultureIgnoreCase)));
            if (OwnerId != null)
                filter = Builders<Place>.Filter.And(filter, Builders<Place>.Filter.Eq(x => x.OwnerId, OwnerId));
            if (SpaceId != null)
                filter = Builders<Place>.Filter.And(filter, Builders<Place>.Filter.Where(x => x.Space != null && x.Space.Id == SpaceId));
            if (PublicAccessType != null)
                filter = Builders<Place>.Filter.And(filter, Builders<Place>.Filter.Eq(x => x.PublicAccessType, PublicAccessType));
            return await Task.FromResult(filter);
        }
    }
}
