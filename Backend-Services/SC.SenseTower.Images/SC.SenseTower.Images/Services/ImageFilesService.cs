using AutoMapper;
using MongoDB.Driver;
using SC.SenseTower.Common.Exceptions;
using SC.SenseTower.Common.Services;
using SC.SenseTower.Images.Data;
using SC.SenseTower.Images.Data.Models;

namespace SC.SenseTower.Images.Services
{
    public class ImageFilesService : BaseDbService
    {
        private new readonly ImagesDbContext context;
        private readonly PlacesService placesService;

        public ImageFilesService(
            ILogger<ImageFilesService> logger,
            IMapper mapper,
            ImagesDbContext context,
            PlacesService placesService) : base(logger, mapper, context)
        {
            this.context = base.context as ImagesDbContext ?? null!;
            this.placesService = placesService;
        }

        public async Task<ImageFile?> Get(Guid id, CancellationToken cancellationToken)
        {
            var filter = Builders<ImageFile>.Filter.Eq(x => x.Id, id);
            return await context.ImageFiles.Find(filter).FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<Guid> Create(Guid id, Guid? userId, string? name, string fileName, CancellationToken cancellationToken)
        {
            var imageFile = new ImageFile
            {
                Id = id,
                FileName = fileName,
                Name = name ?? fileName,
                UserId = userId
            };
            await context.ImageFiles.InsertOneAsync(imageFile, new InsertOneOptions { BypassDocumentValidation = true }, cancellationToken);
            return imageFile.Id;
        }

        public async Task<ImageFile?> Delete(Guid fileId, CancellationToken cancellationToken)
        {
            var imageFile = await Get(fileId, cancellationToken);
            var filter = Builders<ImageFile>.Filter.Eq(x => x.Id, fileId);
            await context.ImageFiles.DeleteOneAsync(filter, new DeleteOptions(), cancellationToken);

            return imageFile;
        }

        public async Task<ImageFile?> Update(Guid id, string name, string accessToken, CancellationToken cancellationToken)
        {
            var image = await Get(id, cancellationToken);

            var filter = Builders<ImageFile>.Filter.Eq(x => x.Id, id);
            var update = Builders<ImageFile>.Update.Set(x => x.Name, name);
            var updateResult = await context.ImageFiles.UpdateOneAsync(filter, update, new UpdateOptions { BypassDocumentValidation = true }, cancellationToken);
            if (updateResult.ModifiedCount == 0)
                throw new ScException("Название изображения не изменено");

            return image;
        }

        public async Task<IEnumerable<ImageFile>> GetByUser(Guid ownerId, bool isAdmin, Guid? userId, CancellationToken cancellationToken)
        {
            var filter = Builders<ImageFile>.Filter.Eq(x => x.UserId, ownerId);
            if (isAdmin)
            {
                filter = Builders<ImageFile>.Filter.Or(filter, Builders<ImageFile>.Filter.Where(x => x.UserId == null));
                if (userId != null)
                    filter = Builders<ImageFile>.Filter.Or(filter, Builders<ImageFile>.Filter.Where(x => x.UserId == userId));
            }
            var sort = Builders<ImageFile>.Sort.Ascending(x => x.Name);
            var result = await context.ImageFiles.Find(filter).Sort(sort).ToListAsync(cancellationToken);
            return result;
        }

        public async Task<IEnumerable<ImageFile>> GetByIds(Guid[] imageIds, CancellationToken cancellationToken)
        {
            var filter = Builders<ImageFile>.Filter.In(x => x.Id, imageIds);
            var result = await context.ImageFiles.Find(filter).ToListAsync(cancellationToken);
            return result;
        }

        public async Task DeleteByUser(Guid userId, CancellationToken cancellationToken)
        {
            var filter = Builders<ImageFile>.Filter.Eq(x => x.UserId, userId);
            await context.ImageFiles.DeleteManyAsync(filter, new DeleteOptions(), cancellationToken);
        }
    }
}
