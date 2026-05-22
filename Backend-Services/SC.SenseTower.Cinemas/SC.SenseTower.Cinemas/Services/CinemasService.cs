using AutoMapper;
using MongoDB.Driver;
using SC.SenseTower.Cinemas.Data;
using SC.SenseTower.Cinemas.Data.Models;
using SC.SenseTower.Common.Extensions;
using SC.SenseTower.Common.Services;

namespace SC.SenseTower.Cinemas.Services
{
    public class CinemasService : BaseDbService
    {
        private new readonly CinemasDbContext context;

        public CinemasService(
            ILogger<CinemasService> logger,
            IMapper mapper,
            CinemasDbContext context) : base(logger, mapper, context)
        {
            this.context = base.context as CinemasDbContext ?? throw new ArgumentNullException(nameof(context), "Не получен контекст данных");
        }

        public async Task<IEnumerable<Cinema>> Get(CancellationToken cancellationToken)
        {
            var filter = Builders<Cinema>.Filter.Empty;
            var result = await context.Cinemas.Find(filter).ToListAsync(cancellationToken);
            return result;
        }

        public async Task<Cinema?> Get(Guid cinemaId, CancellationToken cancellationToken)
        {
            var filter = Builders<Cinema>.Filter.Eq(x => x.Id, cinemaId);
            var result = await context.Cinemas.Find(filter).FirstOrDefaultAsync(cancellationToken);
            return result;
        }

        public async Task<Guid> Create(Cinema cinema, CancellationToken cancellationToken)
        {
            if (cinema.Id == default)
                cinema.Id = Guid.NewGuid();
            await context.Cinemas.InsertOneAsync(cinema, new InsertOneOptions { BypassDocumentValidation = true }, cancellationToken);
            return cinema.Id;
        }

        public async Task Update(Guid id, string name, Space space, CancellationToken cancellationToken)
        {
            var filter = Builders<Cinema>.Filter.Eq(x => x.Id, id);
            var update = Builders<Cinema>.Update.Set(x => x.Name, name).Set(x => x.Space, space);
            await context.Cinemas.UpdateOneAsync(filter, update, new UpdateOptions { BypassDocumentValidation = true }, cancellationToken);
        }

        public async Task Delete(Guid id, CancellationToken cancellationToken)
        {
            var filter = Builders<Cinema>.Filter.Eq(x => x.Id, id);
            await context.Cinemas.DeleteOneAsync(filter, new DeleteOptions(), cancellationToken);
        }

        public async Task<bool> Exists(Guid cinemaId, CancellationToken cancellationToken)
        {
            var filter = Builders<Cinema>.Filter.Eq(x => x.Id, cinemaId);
            var result = await context.Cinemas.Find(filter).AnyAsync(cancellationToken);
            return result;
        }

        public async Task AddAdministrator(Guid cinemaId, UserInfo admin, CancellationToken cancellationToken)
        {
            var cinema = await Get(cinemaId, cancellationToken);
            var admins = cinema?.Administrators.ToList() ?? new List<UserInfo>();
            admins.Add(admin);
            var filter = Builders<Cinema>.Filter.Eq(x => x.Id, cinemaId);
            var update = Builders<Cinema>.Update.Set(x => x.Administrators, admins.ToArray());
            await context.Cinemas.UpdateOneAsync(filter, update, new UpdateOptions { BypassDocumentValidation = true }, cancellationToken);
        }

        public async Task DeleteAdministrator(Guid cinemaId, Guid userId, CancellationToken cancellationToken)
        {
            var filter = Builders<Cinema>.Filter.Eq(x => x.Id, cinemaId);
            var cinema = await context.Cinemas.Find(filter).FirstOrDefaultAsync(cancellationToken);
            var admins = cinema.Administrators.Where(x => x.Id != userId).ToArray();
            var update = Builders<Cinema>.Update.Set(x => x.Administrators, admins.ToArray());
            await context.Cinemas.UpdateOneAsync(filter, update, new UpdateOptions { BypassDocumentValidation = true }, cancellationToken);
        }

        public async Task ReplaceAdministrators(Guid cinemaId, UserInfo[]? admins, CancellationToken cancellationToken)
        {
            var filter = Builders<Cinema>.Filter.Eq(x => x.Id, cinemaId);
            var update = Builders<Cinema>.Update.Set(x => x.Administrators, admins ?? Array.Empty<UserInfo>());
            await context.Cinemas.UpdateOneAsync(filter, update, new UpdateOptions { BypassDocumentValidation = true }, cancellationToken);
        }

        public async Task ClearAdministrator(Guid userId, CancellationToken cancellationToken)
        {
            var filter = Builders<Cinema>.Filter.Where(x => x.Administrators.Any(a => a.Id == userId));
            var cinemas = await context.Cinemas.Find(filter).ToListAsync(cancellationToken);
            cinemas.ForEach(async cinema =>
            {
                cinema.Administrators = cinema.Administrators
                    .Where(x => x.Id != userId)
                    .ToArray();
                var oneFilter = Builders<Cinema>.Filter.Eq(x => x.Id, cinema.Id);
                var update = Builders<Cinema>.Update.Set(x => x.Administrators, cinema.Administrators);
                await context.Cinemas.UpdateOneAsync(filter, update, new UpdateOptions { BypassDocumentValidation = true }, cancellationToken);
            });
        }

        public async Task UpdateSpace(Space space, CancellationToken cancellationToken)
        {
            var filter = Builders<Cinema>.Filter.Eq(x => x.Space.Id, space.Id);
            var update = Builders<Cinema>.Update.Set(x => x.Space, space);
            await context.Cinemas.UpdateManyAsync(filter, update, new UpdateOptions { BypassDocumentValidation = true }, cancellationToken);
        }

        public async Task<Cinema?> GetBySpaceId(Guid spaceId, CancellationToken cancellationToken)
        {
            var filter = Builders<Cinema>.Filter.Eq(x => x.Space.Id, spaceId);
            var result = await context.Cinemas.Find(filter).FirstOrDefaultAsync(cancellationToken);
            return result;
        }

        public async Task DeleteSpace(Guid spaceId, CancellationToken cancellationToken)
        {
            var filter = Builders<Cinema>.Filter.Eq(x => x.Space.Id, spaceId);
            var update = Builders<Cinema>.Update.Set(x => x.Space, new Space { Name = "(удалено)" });
            await context.Cinemas.UpdateManyAsync(filter, update, new UpdateOptions { BypassDocumentValidation = true }, cancellationToken);
        }
    }
}
