using AutoMapper;
using MongoDB.Driver;
using SC.SenseTower.Common.Data;
using SC.SenseTower.Common.Extensions;
using SC.SenseTower.Common.Services;
using SC.SenseTower.Galleries.Constants;
using SC.SenseTower.Galleries.Data;
using SC.SenseTower.Galleries.Data.Models;

namespace SC.SenseTower.Galleries.Services
{
    public class GalleriesService : BaseDbService
    {
        private new readonly GalleriesDbContext context;

        public GalleriesService(
            ILogger<GalleriesService> logger,
            IMapper mapper,
            GalleriesDbContext context) : base(logger, mapper, context)
        {
            this.context = base.context as GalleriesDbContext ?? throw new ArgumentNullException(nameof(context), "Не получен контекст данных");
        }

        public async Task<IEnumerable<Gallery>> Get(AbstractFilter<Gallery>? filter, QuerySorting[] sorting, int offset, int limit, CancellationToken cancellationToken)
        {
            var filters = filter == null ? Builders<Gallery>.Filter.Empty : await filter.Filter(null, cancellationToken);
            var sorts = sorting.ToDefinitions<Gallery>();
            var result = await context.Galleries
                .Find(filters)
                .Sort(sorts)
                .Skip(offset)
                .Limit(limit)
                .ToListAsync(cancellationToken);
            return result;
        }

        public async Task<Gallery?> Get(Guid id, CancellationToken cancellationToken)
        {
            var filter = Builders<Gallery>.Filter.Eq(x => x.Id, id);
            var result = await context.Galleries.Find(filter).FirstOrDefaultAsync(cancellationToken);
            return result;
        }

        public async Task SeedData(CancellationToken cancellationToken)
        {
            foreach(var gallery in FakeData.Galleries)
            {
                if (cancellationToken.IsCancellationRequested)
                    break;

                var entity = new Gallery
                {
                    Id = gallery.Id,
                    Name = gallery.Name,
                    GalleryInfoTable = new GalleryInfoTable
                    {
                        Description = gallery.GalleryInfoTable.Description,
                        IsVisible = gallery.GalleryInfoTable.ShowInformation,
                        Image = new ImageInfo
                        {
                            Id = gallery.GalleryInfoTable.Image.Id,
                            Name = gallery.GalleryInfoTable.Image.Name,
                            FileUrl = gallery.GalleryInfoTable.Image.FileUrl,
                            PreviewUrl = gallery.GalleryInfoTable.Image.PreviewUrl
                        }
                    },
                    Space = new Space
                    {
                        Id = gallery.Space.Id,
                        Name = gallery.Space.SpaceName,
                        SceneName = gallery.Space.SceneName,
                        SpaceType = gallery.Space.SpaceType,
                        SpaceConnectionInfo = new SpaceConnectionInfo
                        {
                            Ip = gallery.Space.SpaceConnectionInfo.Ip,
                            Port = gallery.Space.SpaceConnectionInfo.Port
                        }
                    },
                    Pictures = gallery.PicturesLocation
                        .Select(x => new Picture
                        {
                            Position = x.Key,
                            Image = new GalleryImage
                            {
                                Author = x.Value.Author,
                                Description = x.Value.Description ?? string.Empty,
                                Name = x.Value.Name,
                                PassepartoutWidthInMeters = x.Value.PassepartoutWidthInMeters,
                                PictureWidthInMeters = x.Value.PictureWidthInMeters,
                                Image = new ImageInfo
                                {
                                    Id = x.Value.Image.Id,
                                    Name = x.Value.Image.Name,
                                    FileUrl = x.Value.Image.FileUrl,
                                    PreviewUrl = x.Value.Image.PreviewUrl
                                }
                            }
                        })
                        .ToArray()
                };
                await Create(entity, cancellationToken);
            }
        }

        public async Task<Guid> Create(Gallery entity, CancellationToken cancellationToken)
        {
            if (entity.Id == default)
                entity.Id = Guid.NewGuid();
            await context.Galleries.InsertOneAsync(entity, new InsertOneOptions { BypassDocumentValidation = true }, cancellationToken);
            return entity.Id;
        }

        public async Task Update(Gallery gallery, CancellationToken cancellationToken)
        {
            var filter = Builders<Gallery>.Filter.Eq(x => x.Id, gallery.Id);
            await context.Galleries.ReplaceOneAsync(filter, gallery, new ReplaceOptions { BypassDocumentValidation = true }, cancellationToken);
        }

        public async Task Delete(Guid id, CancellationToken cancellationToken)
        {
            var filter = Builders<Gallery>.Filter.Eq(x => x.Id, id);
            await context.Galleries.DeleteOneAsync(filter, new DeleteOptions(), cancellationToken);
        }

        public async Task DeletePicture(Guid galleryId, int position, CancellationToken cancellationToken)
        {
            var filter = Builders<Gallery>.Filter.Eq(x => x.Id, galleryId);
            var gallery = await context.Galleries.Find(filter).FirstOrDefaultAsync(cancellationToken);
            gallery.Pictures = gallery.Pictures
                .Where(x => x.Position != position)
                .ToArray();
            await context.Galleries.ReplaceOneAsync(filter, gallery, new ReplaceOptions { BypassDocumentValidation = true }, cancellationToken);
        }

        public async Task<bool> Exists(Guid id, CancellationToken cancellationToken)
        {
            var filter = Builders<Gallery>.Filter.Eq(x => x.Id, id);
            var result = await context.Galleries.Find(filter).AnyAsync(cancellationToken);
            return result;
        }

        public async Task ReplacePictures(Guid galleryId, Picture[] pictures, CancellationToken cancellationToken)
        {
            var filter = Builders<Gallery>.Filter.Eq(x => x.Id, galleryId);
            var update = Builders<Gallery>.Update.Set(x => x.Pictures, pictures);
            await context.Galleries.UpdateOneAsync(filter, update, new UpdateOptions { BypassDocumentValidation = true }, cancellationToken);
        }

        public async Task<Gallery?> GetBySpace(Guid spaceId, CancellationToken cancellationToken)
        {
            var filter = Builders<Gallery>.Filter.Eq(x => x.Space.Id, spaceId);
            var result = await context.Galleries.Find(filter).FirstOrDefaultAsync(cancellationToken);
            return result;
        }

        public async Task UpdateSpace(Space space, CancellationToken cancellationToken)
        {
            var filter = Builders<Gallery>.Filter.Eq(x => x.Space.Id, space.Id);
            var update = Builders<Gallery>.Update.Set(x => x.Space, space);
            await context.Galleries.UpdateManyAsync(filter, update, new UpdateOptions { BypassDocumentValidation = true }, cancellationToken);
        }

        public async Task DeleteSpace(Guid spaceId, CancellationToken cancellationToken)
        {
            var filter = Builders<Gallery>.Filter.Eq(x => x.Space.Id, spaceId);
            var update = Builders<Gallery>.Update.Set(x => x.Space, new Space { Name = "(удалено)" });
            await context.Galleries.UpdateManyAsync(filter, update, new UpdateOptions { BypassDocumentValidation = true }, cancellationToken);
        }
    }
}
