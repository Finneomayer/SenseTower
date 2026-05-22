using AutoMapper;
using Flurl.Util;
using MongoDB.Bson;
using MongoDB.Driver;
using SC.SenseTower.Common.Extensions;
using SC.SenseTower.Common.Models;
using SC.SenseTower.Common.Services;
using SC.SenseTower.Halls.Constants;
using SC.SenseTower.Halls.Data;
using SC.SenseTower.Halls.Data.Models;
using SC.SenseTower.Halls.Dto.Halls;
using System.Linq;

namespace SC.SenseTower.Halls.Services
{
    public class HallsService : BaseDbService
    {
        private new HallsDbContext context;

        public HallsService(
            ILogger<HallsService> logger,
            IMapper mapper,
            HallsDbContext context) : base(logger, mapper, context)
        {
            this.context = base.context as HallsDbContext ?? null!;
        }

        /// <summary>
        /// Удаление холла по его идентификатору.
        /// </summary>
        /// <param name="hallId">Идентификатор холла.</param>
        /// <param name="cancellationToken">Токен прерывания выполнения.</param>
        /// <returns>True, если операция выполнена успешно.</returns>
        public async Task<bool> Delete(Guid hallId, CancellationToken cancellationToken)
        {
            var filter = Builders<Hall>.Filter.Eq(x => x.Id, hallId);
            var result = await context.Halls.DeleteOneAsync(filter, cancellationToken);
            if (result.DeletedCount == 0)
                return false;
            return true;
        }

        /// <summary>
        /// Обновление данных холла (в текущей реализации только название).
        /// </summary>
        /// <param name="hall">Новые данные холла.</param>
        /// <param name="cancellationToken">Токен прерывания выполнения.</param>
        /// <returns>True, если операция выполнена успешно.</returns>
        public async Task<bool> Update(Hall hall, CancellationToken cancellationToken)
        {
            var filter = Builders<Hall>.Filter.Eq(r => r.Id, hall.Id);
            var result = await context.Halls.ReplaceOneAsync(filter, hall, new ReplaceOptions { BypassDocumentValidation = true }, cancellationToken);
            if (result.ModifiedCount == 0)
                return false;
            return true;
        }

        /// <summary>
        /// Создание нового холла.
        /// </summary>
        /// <param name="hall">Новая сущность холла.</param>
        /// <param name="cancellationToken">Токен прерывания выполнения.</param>
        /// <returns>Идентификатор созданного холла.</returns>
        public async Task<Guid> Create(Hall hall, CancellationToken cancellationToken)
        {
            if (hall.Id == default)
                hall.Id = Guid.NewGuid();
            await context.Halls.InsertOneAsync(hall, new InsertOneOptions { BypassDocumentValidation = true }, cancellationToken);
            return hall.Id;
        }

        /// <summary>
        /// Получение холла из БД по его идентификатору.
        /// </summary>
        /// <param name="hallId">Идентификатор холла.</param>
        /// <param name="cancellationToken">Токен прерывания выполнения.</param>
        /// <returns>Сущность холла.</returns>
        public async Task<Hall?> Get(Guid hallId, CancellationToken cancellationToken)
        {
            var filter = Builders<Hall>.Filter.Eq(r => r.Id, hallId);
            var result = await context.Halls.Find(filter).FirstOrDefaultAsync(cancellationToken);
            return result;
        }

        /// <summary>
        /// Получение списка доступных холлов.
        /// </summary>
        /// <param name="userId">Идентификатор пользователя (зарезервирован для возможной в будущем фильтрации по правам).</param>
        /// <param name="cancellationToken">Токен прерывания выполнения.</param>
        /// <returns>Массив холлов.</returns>
        public async Task<Hall[]> GetList(Guid? userId, CancellationToken cancellationToken)
        {
            //todo: userId здесь для возможной последующей фильтрации по правам пользователя на холлы
            var result = await context.Halls.Find(new BsonDocument()).ToListAsync(cancellationToken);
            return result?.ToArray() ?? Array.Empty<Hall>();
        }

        public async Task<IEnumerable<LookupItemDto<Guid>>> Lookup(Guid[] hallIds, CancellationToken cancellationToken)
        {
            var filter = hallIds.Length > 0 ? Builders<Hall>.Filter.In(x => x.Id, hallIds) : Builders<Hall>.Filter.Empty;
            var data = await context.Halls.Find(filter).ToListAsync(cancellationToken);
            return Mapper.Map<LookupItemDto<Guid>[]>(data.OrderBy(x => x.Name));
        }

        public async Task<Hall?> GetByPlaceId(Guid placeId, CancellationToken cancellationToken)
        {
            var filter = Builders<Hall>.Filter.Where(x => x.UserPlaces.Any(p => p.Id == placeId));
            var result = await context.Halls.Find(filter).FirstOrDefaultAsync(cancellationToken);
            return result;
        }

        public async Task ClearUserPlaces(Guid[] placeIds, CancellationToken cancellationToken)
        {
            var halls = await context.Halls.Find(x => true).ToListAsync(cancellationToken);
            foreach (var hall in halls)
            {
                var places = hall.UserPlaces
                    .Where(x => !placeIds.Contains(x.Id))
                    .ToArray();
                if (places.Length != hall.UserPlaces.Length)
                {
                    var filter = Builders<Hall>.Filter.Eq(x => x.Id, hall.Id);
                    var update = Builders<Hall>.Update.Set(x => x.UserPlaces, places);
                    await context.Halls.UpdateOneAsync(filter, update, new UpdateOptions { BypassDocumentValidation = true }, cancellationToken);
                }    
            }
        }

        public async Task UpdateUserPlace(UserPlaceDto place, CancellationToken cancellationToken)
        {
            var halls = await context.Halls.Find(x => true).ToListAsync(cancellationToken);
            foreach (var hall in halls.Where(x => x.UserPlaces.Any(p => p.Id == place.Id)))
            {
                var hallPlace = hall.UserPlaces.First(x => x.Id == place.Id);
                Mapper.Map(place, hallPlace);
                var filter = Builders<Hall>.Filter.Eq(x => x.Id, hall.Id);
                var update = Builders<Hall>.Update.Set(x => x.UserPlaces, hall.UserPlaces);
                await context.Halls.UpdateOneAsync(filter, update, new UpdateOptions { BypassDocumentValidation = true }, cancellationToken);
            }
        }

        public async Task ClearUser(Guid userId, CancellationToken cancellationToken)
        {
            var filter = Builders<Hall>.Filter.Where(x => x.Spaces.Any(p => p.SpaceOwner != null && p.SpaceOwner.UserId == userId));
            var halls = await context.Halls.Find(filter).ToListAsync(cancellationToken);
            halls.ForEach(async hall =>
            {
                hall.Spaces
                    .Where(x => x.SpaceOwner != null && x.SpaceOwner.UserId == userId)
                    .ForEach(x =>
                    {
                        x.SpaceOwner = null;
                    });
                var oneFilter = Builders<Hall>.Filter.Eq(x => x.Id, hall.Id);
                var update = Builders<Hall>.Update.Set(x => x.Spaces, hall.Spaces);
                await context.Halls.UpdateOneAsync(filter, update, new UpdateOptions { BypassDocumentValidation = true }, cancellationToken);
            });
        }

        public async Task UpdateSpace(LocalSpace space, CancellationToken cancellationToken)
        {
            var filter = Builders<Hall>.Filter.Where(x => x.Space != null && x.Space.Id == space.Id);
            var update = Builders<Hall>.Update.Set(x => x.Space, space);
            await context.Halls.UpdateManyAsync(filter, update, new UpdateOptions { BypassDocumentValidation = true }, cancellationToken);

            filter = Builders<Hall>.Filter.Where(x => x.Spaces.Any(p => p.Id == space.Id));
            var halls = await context.Halls.Find(filter).ToListAsync(cancellationToken);
            foreach (var hall in halls)
            {
                for (var i = 0; i < (hall.Spaces?.Length ?? 0); i++)
                {
                    if (hall.Spaces[i].Id == space.Id)
                    {
                        hall.Spaces[i] = Mapper.Map<LocalSpace>(space);
                    }
                }
                filter = Builders<Hall>.Filter.Eq(x => x.Id, hall.Id);
                update = Builders<Hall>.Update.Set(x => x.Spaces, hall.Spaces);
                var result = await context.Halls.UpdateOneAsync(filter, update, new UpdateOptions { BypassDocumentValidation = true }, cancellationToken);
            }
        }

        public async Task ClearSpace(Guid spaceId, CancellationToken cancellationToken)
        {
            var filter = Builders<Hall>.Filter.Where(x => x.Space != null && x.Space.Id == spaceId);
            var update = Builders<Hall>.Update.Set(x => x.Space, null);
            await context.Halls.UpdateManyAsync(filter, update, new UpdateOptions { BypassDocumentValidation = true }, cancellationToken);

            filter = Builders<Hall>.Filter.Where(x => x.Spaces.Any(p => p.Id == spaceId));
            var halls = await context.Halls.Find(filter).ToListAsync(cancellationToken);
            foreach (var hall in halls)
            {
                var spaces = hall.Spaces.Where(x => x.Id != spaceId).ToArray();
                filter = Builders<Hall>.Filter.Eq(x => x.Id, hall.Id);
                update = Builders<Hall>.Update.Set(x => x.Spaces, spaces);
                await context.Halls.UpdateOneAsync(filter, update, new UpdateOptions { BypassDocumentValidation = true }, cancellationToken);
            }
        }

        public async Task DeletePlaces(Guid[] placeIds, CancellationToken cancellationToken)
        {
            var filter = Builders<Hall>.Filter.Where(x => x.UserPlaces.Any(p => placeIds.Contains(p.Id)));
            var halls = await context.Halls.Find(filter).ToListAsync(cancellationToken);
            foreach (var hall in halls)
            {
                var places = hall.UserPlaces.Where(x => !placeIds.Contains(x.Id)).ToArray();
                if (places.Length != hall.UserPlaces.Length)
                {
                    filter = Builders<Hall>.Filter.Eq(x => x.Id, hall.Id);
                    var update = Builders<Hall>.Update.Set(x => x.UserPlaces, places);
                    await context.Halls.UpdateOneAsync(filter, update, new UpdateOptions { BypassDocumentValidation = true }, cancellationToken);
                }
            }
        }

        public async Task AddSpace(Guid hallId, LocalSpace space, CancellationToken cancellationToken)
        {
            var filter = Builders<Hall>.Filter.Eq(x => x.Id, hallId);
            var hall = await context.Halls.Find(filter).FirstAsync(cancellationToken);
            var spaces = hall.Spaces.ToList();
            spaces.Add(space);
            var update = Builders<Hall>.Update.Set(x => x.Spaces, spaces.ToArray());
            await context.Halls.UpdateOneAsync(filter, update, new UpdateOptions { BypassDocumentValidation = true }, cancellationToken);
        }

        public async Task RemoveSpace(Guid hallId, Guid spaceId, CancellationToken cancellationToken)
        {
            var filter = Builders<Hall>.Filter.Eq(x => x.Id, hallId);
            var hall = await context.Halls.Find(filter).FirstAsync(cancellationToken);
            var spaces = hall.Spaces
                .Where(x => x.Id != spaceId)
                .ToArray();
            var update = Builders<Hall>.Update.Set(x => x.Spaces, spaces);
            await context.Halls.UpdateOneAsync(filter, update, new UpdateOptions { BypassDocumentValidation = true }, cancellationToken);
        }
    }
}
