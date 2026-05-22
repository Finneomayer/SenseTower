using MongoDB.Bson;
using MongoDB.Driver;
using SC.SenseTower.Common.Data;
using SC.SenseTower.Common.Services;
using System.Text.RegularExpressions;

namespace SC.SenseTower.Galleries.Cqrs.GalleryList
{
    public class GalleryListFilter : AbstractFilter<Data.Models.Gallery>
    {
        /// <summary>
        /// Начальные символы названия галереи.
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Признак видимости инфо-табло.
        /// </summary>
        public bool? IsVisible { get; set; }

        public override async Task<FilterDefinition<Data.Models.Gallery>> Filter(BaseDbService? service, CancellationToken cancellationToken)
        {
            var filter = Builders<Data.Models.Gallery>.Filter.Empty;
            if (!string.IsNullOrWhiteSpace(Name))
            {
                var queryExpression = new BsonRegularExpression(new Regex('^' + Name + ".*", RegexOptions.IgnoreCase));
                filter = Builders<Data.Models.Gallery>.Filter.Regex(x => x.Name, queryExpression);
            }
            if (IsVisible.HasValue)
                filter = Builders<Data.Models.Gallery>.Filter.And(filter, Builders<Data.Models.Gallery>.Filter.Eq(x => x.GalleryInfoTable.IsVisible, IsVisible));
            return await Task.FromResult(filter);
        }
    }
}
