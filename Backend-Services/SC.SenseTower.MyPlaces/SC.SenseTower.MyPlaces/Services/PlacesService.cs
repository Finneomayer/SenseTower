using AutoMapper;
using MongoDB.Driver;
using SC.SenseTower.Common.Data;
using SC.SenseTower.Common.Enums;
using SC.SenseTower.Common.Exceptions;
using SC.SenseTower.Common.Extensions;
using SC.SenseTower.Common.Models;
using SC.SenseTower.Common.Services;
using SC.SenseTower.MyPlaces.Cqrs.Requests;
using SC.SenseTower.MyPlaces.Data;
using SC.SenseTower.MyPlaces.Data.Models;
using SC.SenseTower.MyPlaces.Dto.Places;

namespace SC.SenseTower.MyPlaces.Services
{
    public class PlacesService : BaseDbService
    {
        private new MyPlacesDbContext context { get; }
     
        private readonly HallsService hallsService;

        public PlacesService(
            ILogger<PlacesService> logger,
            IMapper mapper,
            MyPlacesDbContext context,
            HallsService hallsService) : base(logger, mapper, context)
        {
            this.context = base.context as MyPlacesDbContext ?? null!;
            this.hallsService = hallsService;
        }

        public async Task<Place?> Get(Guid id, CancellationToken cancellationToken)
        {
            var filter = Builders<Place>.Filter.Eq(x => x.Id, id);
            var result = await context.Places.Find(filter).FirstOrDefaultAsync(cancellationToken);
            return result;
        }

        public async Task<Place[]> GetUserPlaces(Guid userId, CancellationToken cancellationToken)
        {
            var filter = Builders<Place>.Filter.Eq(r => r.OwnerId, userId);
            var result = await context.Places.Find(filter).ToListAsync(cancellationToken);
            return result.ToArray() ?? Array.Empty<Place>();
        }

        public async Task<IEnumerable<Place>?> GetAllPlaces(CancellationToken cancellationToken)
        {
            var result = await context.Places.Find(x => true).ToListAsync(cancellationToken);
            return result;
        }

        public async Task<bool> DeleteByOwner(string accessToken, Guid? ownerId, CancellationToken cancellationToken)
        {
            var filter = Builders<Place>.Filter.Eq(r => r.OwnerId, ownerId);
            await context.Places.DeleteManyAsync(filter, cancellationToken);
            return true;
        }

        public async Task<PlaceDto> GetFullPlaceById(Guid placeId, CancellationToken cancellationToken)
        {
            var filter = Builders<Place>.Filter.Eq(r => r.Id, placeId);
            var data = await context.Places.Find(filter).FirstOrDefaultAsync(cancellationToken);
            var result = Mapper.Map<PlaceDto>(data);
            return result;
        }

        public async Task<IEnumerable<Place>> GetPlacesByIds(Guid[] placeIds, CancellationToken cancellationToken)
        {
            var filter = Builders<Place>.Filter.In(r => r.Id, placeIds);
            var data = await context.Places.Find(filter).ToListAsync(cancellationToken);
            return data.OrderBy(x => x.PlaceName);
        }

        public async Task<Guid> Create(Guid id, string placeName, int placeNumber, AccessType? publicAccessType, Guid? ownerId, Guid? doorImageId, Space? space, CancellationToken cancellationToken)
        {
            var place = new Place
            {
                Id = id == default ? Guid.NewGuid() : id,
                OwnerId = ownerId,
                PlaceName = placeName,
                PlaceNumber = placeNumber,
                PublicAccessType = publicAccessType ?? AccessType.Public,
                DoorImageId = doorImageId,
                Space = space
            };
            await context.Places.InsertOneAsync(place, new InsertOneOptions { BypassDocumentValidation = false }, cancellationToken);
            return place.Id;
        }

        public async Task ReplaceImages(Guid id, Picture[]? images, CancellationToken cancellationToken)
        {
            var filter = Builders<Place>.Filter.Eq(x => x.Id, id);
            var update = Builders<Place>.Update.Set(x => x.Images, images);
            await context.Places.UpdateOneAsync(filter, update, new UpdateOptions { BypassDocumentValidation = false }, cancellationToken);
        }

        public async Task Update(
            Guid id,
            Guid? ownerId,
            string placeName,
            int placeNumber,
            AccessType accessType,
            Guid? doorImageId,
            Space? space,
            CancellationToken cancellationToken)
        {
            var filter = Builders<Place>.Filter.Eq(x => x.Id, id);
            var update = Builders<Place>.Update
                .Set(x => x.OwnerId, ownerId)
                .Set(x => x.PlaceName, placeName)
                .Set(x => x.PlaceNumber, placeNumber)
                .Set(x => x.PublicAccessType, accessType)
                .Set(x => x.DoorImageId, doorImageId)
                .Set(x => x.Space, space);
            var updateResult = await context.Places.UpdateOneAsync(filter, update, new UpdateOptions { BypassDocumentValidation = false }, cancellationToken);
        }

        public async Task UpdateAccessType(string accessToken, Guid id, AccessType accessType, CancellationToken cancellationToken)
        {
            var filter = Builders<Place>.Filter.Eq(x => x.Id, id);
            var update = Builders<Place>.Update.Set(x => x.PublicAccessType, accessType);
            await context.Places.UpdateOneAsync(filter, update, new UpdateOptions { BypassDocumentValidation = false }, cancellationToken);
        }

        public async Task<LookupItemDto<Guid>[]?> Lookup(Guid[] placeIds, CancellationToken cancellationToken)
        {
            var filter = Builders<Place>.Filter.In(x => x.Id, placeIds);
            var data = await context.Places.Find(filter).ToListAsync(cancellationToken);
            return Mapper.Map<LookupItemDto<Guid>[]?>(data.OrderBy(x => x.PlaceName));
        }

        public async Task Delete(string accessToken, Guid placeId, CancellationToken cancellationToken)
        {
            var filter = Builders<Place>.Filter.Eq(x => x.Id, placeId);
            await context.Places.DeleteOneAsync(filter, new DeleteOptions(), cancellationToken);
        }

        public async Task UpdateDoorImage(string accessToken, Guid placeId, Guid? doorImageId, CancellationToken cancellationToken)
        {
            var filter = Builders<Place>.Filter.Eq(x => x.Id, placeId);
            var update = Builders<Place>.Update.Set(x => x.DoorImageId, doorImageId);
            await context.Places.UpdateOneAsync(filter, update, new UpdateOptions { BypassDocumentValidation = true }, cancellationToken);
        }

        public async Task<Place?> GetBySpaceId(Guid spaceId, CancellationToken cancellationToken)
        {
            var filter = Builders<Place>.Filter.Where(x => x.Space != null && x.Space.Id == spaceId);
            var result = await context.Places.Find(filter).FirstOrDefaultAsync(cancellationToken);
            return result;
        }

        public async Task<bool> DeleteImage(Guid placeId, Guid imageId, CancellationToken cancellationToken)
        {
            var filter = Builders<Place>.Filter.Eq(x => x.Id, placeId);
            var place = await context.Places.Find(filter).FirstOrDefaultAsync(cancellationToken);
            var images = place.Images?
                .Where(x => x.Image.Id != imageId)
                .ToArray() ?? Array.Empty<Picture>();
            if (images.Length != (place.Images?.Length ?? 0))
            {
                var update = Builders<Place>.Update.Set(x => x.Images, images);
                await context.Places.UpdateOneAsync(filter, update, new UpdateOptions { BypassDocumentValidation = true }, cancellationToken);
                return true;
            }
            return false;
        }

        public async Task<bool> Exists(Guid id, CancellationToken cancellationToken)
        {
            var filter = Builders<Place>.Filter.Eq(x => x.Id, id);
            var result = await context.Places.Find(filter).AnyAsync(cancellationToken);
            return result;
        }

        public async Task<Guid> Create(Place place, CancellationToken cancellationToken)
        {
            if (place.Id == default)
                place.Id = Guid.NewGuid();
            await context.Places.InsertOneAsync(place, new InsertOneOptions { BypassDocumentValidation = true }, cancellationToken);
            return place.Id;
        }

        public async Task ClearUser(Guid userId, CancellationToken cancellationToken)
        {
            var filter = Builders<Place>.Filter.Eq(x => x.OwnerId, userId);
            var update = Builders<Place>.Update
                .Set(x => x.OwnerId, null)
                .Set(x => x.DoorImageId, null)
                .Set(x => x.Images, Array.Empty<Picture>());
            await context.Places.UpdateManyAsync(filter, update, new UpdateOptions { BypassDocumentValidation = true }, cancellationToken);
        }

        public async Task Update(Place place, CancellationToken cancellationToken)
        {
            var filter = Builders<Place>.Filter.Eq(x => x.Id, place.Id);
            await context.Places.ReplaceOneAsync(filter, place, new ReplaceOptions { BypassDocumentValidation = true }, cancellationToken);
        }

        public async Task<IEnumerable<Place>> GetByOwnerId(Guid? ownerId, CancellationToken cancellationToken)
        {
            var filter = Builders<Place>.Filter.Eq(r => r.OwnerId, ownerId);
            var result = await context.Places.Find(filter).ToListAsync(cancellationToken);
            return result;
        }

        public async Task<IEnumerable<Place>> Get(AbstractFilter<Place> filters, QuerySorting[] sorting, int offset, int limit, CancellationToken cancellationToken)
        {
            var filter = await filters.Filter(null, cancellationToken);
            var query = context.Places.Find(filter).Sort(sorting.ToDefinitions<Place>());
            if (offset > 0)
                query = query.Skip(offset);
            if (limit > 0)
                query = query.Limit(limit);
            var result = await query.ToListAsync(cancellationToken);
            return result;
        }

        public async Task<long> Count(AbstractFilter<Place> filters, CancellationToken cancellationToken)
        {
            var filter = await filters.Filter(null, cancellationToken);
            var result = await context.Places.Find(filter).CountDocumentsAsync(cancellationToken);
            return result;
        }

        public async Task<IEnumerable<Guid>> GetAllocatedSpaceIds(CancellationToken cancellationToken)
        {
            var places = await context.Places.Find(Builders<Place>.Filter.Empty).ToListAsync(cancellationToken);
            var result = places
                .Where(x => x.Space != null)
                .Select(x => x.Space.Id)
                .Distinct()
                .ToArray();
            return result;
        }

        public async Task<Place?> GetByNumber(int number, CancellationToken cancellationToken)
        {
            var filter = Builders<Place>.Filter.Eq(x => x.PlaceNumber, number);
            var result = await context.Places.Find(filter).FirstOrDefaultAsync(cancellationToken);
            return result;
        }
    }
}
