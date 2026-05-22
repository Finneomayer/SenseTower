using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using SC.SenseTower.Common.Enums;
using SC.SenseTower.Common.Exceptions;
using SC.SenseTower.Common.Models;
using SC.SenseTower.Common.Services;
using SC.SenseTower.Spaces.Data;
using SC.SenseTower.Spaces.Data.Models;

namespace SC.SenseTower.Spaces.Services
{
    public class SpacesService : BaseDbService, ISpacesService
    {
        private new readonly SpacesDbContext context;

        public SpacesService(ILogger<SpacesService> logger, IMapper mapper, SpacesDbContext context)
            : base(logger, mapper, context)
        {
            this.context = base.context as SpacesDbContext ?? null!;
        }

        public async Task<Space?> Get(Guid id, CancellationToken cancellationToken)
        {
            var filter = Builders<Space>.Filter.Eq(x => x.Id, id);
            var result = await context.Spaces.Find(filter).FirstOrDefaultAsync(cancellationToken);
            return result;
        }

        public async Task<IEnumerable<Space>?> GetByOwnerId(Guid ownerId, CancellationToken cancellationToken)
        {
            var filter = Builders<Space>.Filter.Where(x => x.SpaceOwner != null && x.SpaceOwner.UserId == ownerId);
            var result = await context.Spaces.Find(filter).ToListAsync(cancellationToken);
            return result;
        }

        public async Task<Guid> Create(Space space, CancellationToken cancellationToken)
        {
            if (space.Id == default)
                space.Id = Guid.NewGuid();
            await context.Spaces.InsertOneAsync(space, new InsertOneOptions { BypassDocumentValidation = true }, cancellationToken);
            return space.Id;
        }

        public async Task Update(Space space, CancellationToken cancellationToken)
        {
            var filter = Builders<Space>.Filter.Eq(x => x.Id, space.Id);
            await context.Spaces.FindOneAndReplaceAsync(filter, space, new FindOneAndReplaceOptions<Space, Space> { IsUpsert = false, BypassDocumentValidation = true }, cancellationToken);
        }

        public async Task Delete(Guid id, CancellationToken cancellationToken)
        {
            var filter = Builders<Space>.Filter.Eq(x => x.Id, id);
            var deleteResult = await context.Spaces.DeleteOneAsync(filter, new DeleteOptions(), cancellationToken);
            if (deleteResult.DeletedCount != 1)
                throw new ScException("Пространство не удалено");
        }

        public async Task<IEnumerable<Space>> GetAll(CancellationToken cancellationToken)
        {
            var filter = Builders<Space>.Filter.Empty;
            var result = await context.Spaces.Find(filter, new FindOptions { NoCursorTimeout = true }).ToListAsync(cancellationToken);
            return result;
        }

        public async Task<LookupItemDto<Guid>[]> Lookup(SpaceType? spaceType, CancellationToken cancellationToken)
        {
            var filter = spaceType != null
                ? Builders<Space>.Filter.Eq(x => x.SpaceType, spaceType)
                : Builders<Space>.Filter.Empty;
            var data = await context.Spaces.Find(filter).ToListAsync(cancellationToken);
            var result = Mapper.Map<LookupItemDto<Guid>[]?>(data.OrderBy(x => x.SpaceName));
            return result ?? Array.Empty<LookupItemDto<Guid>>();
        }

        public async Task ReplaceImages(Guid spaceId, Picture[] pictures, CancellationToken cancellationToken)
        {
            var filter = Builders<Space>.Filter.Eq(x => x.Id, spaceId);
            var update = Builders<Space>.Update.Set(x => x.Images, pictures);
            await context.Spaces.UpdateOneAsync(filter, update, new UpdateOptions { BypassDocumentValidation = false }, cancellationToken);
        }

        public async Task<bool> DeleteImage(Guid spaceId, Guid imageId, CancellationToken cancellationToken)
        {
            var filter = Builders<Space>.Filter.Eq(x => x.Id, spaceId);
            var place = await context.Spaces.Find(filter).FirstOrDefaultAsync(cancellationToken);
            var images = place.Images?
                .Where(x => x.Image.Id != imageId)
                .ToArray() ?? Array.Empty<Picture>();
            if (images.Length != (place.Images?.Length ?? 0))
            {
                var update = Builders<Space>.Update.Set(x => x.Images, images);
                await context.Spaces.UpdateOneAsync(filter, update, new UpdateOptions { BypassDocumentValidation = true }, cancellationToken);
                return true;
            }
            return false;
        }

        public async Task ClearOwner(Guid userId, CancellationToken cancellationToken)
        {
            var filter = Builders<Space>.Filter.Where(x => x.SpaceOwner != null && x.SpaceOwner.UserId == userId);
            var update = Builders<Space>.Update.Set(x => x.SpaceOwner, null);
            await context.Spaces.UpdateManyAsync(filter, update, new UpdateOptions { BypassDocumentValidation = true }, cancellationToken);
        }
    }
}
