using Microsoft.Extensions.Options;
using MongoDB.Driver;
using SC.SenseTower.Images.Constants;
using SC.SenseTower.Images.Data.Models;
using SC.SenseTower.Common.Data;
using SC.SenseTower.Common.Models;

namespace SC.SenseTower.Images.Data
{
    public class ImagesDbContext : BaseDbContext
    {
        private IMongoCollection<ImageFile>? imageFiles = null;
        public IMongoCollection<ImageFile> ImageFiles => imageFiles ??= GetDbCollection<ImageFile>(DbConstants.COLLECTION_IMAGE_FILES);

        public ImagesDbContext(IOptions<MongoDbConfig> options) : base(options)
        {
        }
    }
}
