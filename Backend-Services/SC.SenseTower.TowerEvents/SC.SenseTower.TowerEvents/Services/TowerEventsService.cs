using AutoMapper;
using MongoDB.Driver;
using SC.SenseTower.Common.Data;
using SC.SenseTower.Common.Enums;
using SC.SenseTower.Common.Extensions;
using SC.SenseTower.Common.Services;
using SC.SenseTower.TowerEvents.Cqrs.AdminTowerEventList;
using SC.SenseTower.TowerEvents.Cqrs.TowerEventList;
using SC.SenseTower.TowerEvents.Data;
using SC.SenseTower.TowerEvents.Data.Models;

namespace SC.SenseTower.TowerEvents.Services
{
    public class TowerEventsService : BaseDbService
    {
        private new readonly TowerEventsDbContext context;

        public TowerEventsService(
            ILogger<TowerEventsService> logger,
            IMapper mapper,
            TowerEventsDbContext context) : base(logger, mapper, context)
        {
            this.context = base.context as TowerEventsDbContext ?? null!;
        }

        public async Task<IEnumerable<TowerEvent>> GetList(TowerEventListFilter? requestFilter, int? limit, CancellationToken cancellationToken)
        {
            var sort = Builders<TowerEvent>.Sort.Descending(x => x.Date);
            var filter = requestFilter != null
                ? await requestFilter.Filter(null, cancellationToken)
                : Builders<TowerEvent>.Filter.Gt(x => x.UpTo, DateTime.UtcNow);
            var result = await (limit == null ? context.TowerEvents.Find(filter).Sort(sort) : context.TowerEvents.Find(filter).Sort(sort).Limit(limit)).ToListAsync(cancellationToken);
            return result;
        }

        public async Task<TowerEvent?> Get(Guid eventId, CancellationToken cancellationToken)
        {
            var filter = Builders<TowerEvent>.Filter.Eq(x => x.Id, eventId);
            var result = await context.TowerEvents.Find(filter).FirstOrDefaultAsync(cancellationToken);
            return result;
        }

        public async Task<IEnumerable<TowerEvent>> Get(AbstractFilter<TowerEvent>? filter, QuerySorting[] sorting, int offset, int limit, CancellationToken cancellationToken)
        {
            var filters = filter == null ? Builders<TowerEvent>.Filter.Empty : await filter.Filter(null, cancellationToken);
            var sorts = sorting.ToDefinitions<TowerEvent>();
            var result = await context.TowerEvents
                .Find(filters)
                .Sort(sorts)
                .Skip(offset)
                .Limit(limit)
                .ToListAsync(cancellationToken);
            return result;
        }

        public async Task<Guid> Create(TowerEvent towerEvent, CancellationToken cancellationToken)
        {
            if (towerEvent.Id == default)
                towerEvent.Id = Guid.NewGuid();
            await context.TowerEvents.InsertOneAsync(towerEvent, new InsertOneOptions { BypassDocumentValidation = true }, cancellationToken);
            return towerEvent.Id;
        }

        public async Task SellTicket(Guid eventId, Guid ticketId, Guid userId, CancellationToken cancellationToken)
        {
            var filter = Builders<TowerEvent>.Filter.Eq(x => x.Id, eventId);
            var towerEvent = await context.TowerEvents.Find(filter).FirstOrDefaultAsync(cancellationToken);
            if (towerEvent == null)
                return;

            var tickets = towerEvent.SoldTickets.ToList();
            tickets.Add(new Ticket { Id = ticketId, UserId = userId });

            var update = Builders<TowerEvent>.Update.Set(x => x.SoldTickets, tickets.ToArray());
            await context.TowerEvents.UpdateOneAsync(filter, update, new UpdateOptions { BypassDocumentValidation = true }, cancellationToken);
        }

        public async Task SetTotalTickets(Guid eventId, int totalTickets, CancellationToken cancellationToken)
        {
            var filter = Builders<TowerEvent>.Filter.Eq(x => x.Id, eventId);
            var update = Builders<TowerEvent>.Update.Set(x => x.TotalTickets, totalTickets);
            await context.TowerEvents.UpdateOneAsync(filter, update, new UpdateOptions { BypassDocumentValidation = true }, cancellationToken);
        }

        public async Task UpdateSpace(Space space, CancellationToken cancellationToken)
        {
            var filter = Builders<TowerEvent>.Filter.Where(x => x.Space != null && x.Space.Id == space.Id);
            var update = Builders<TowerEvent>.Update.Set(x => x.Space, space);
            await context.TowerEvents.UpdateManyAsync(filter, update, new UpdateOptions { BypassDocumentValidation = true }, cancellationToken);
        }

        public async Task<bool> Exists(Guid eventId, CancellationToken cancellationToken)
        {
            var filter = Builders<TowerEvent>.Filter.Eq(x => x.Id, eventId);
            var result = await context.TowerEvents.Find(filter).AnyAsync(cancellationToken);
            return result;
        }

        public async Task UpdateState(Guid eventId, TowerEventState state, CancellationToken cancellationToken)
        {
            var filter = Builders<TowerEvent>.Filter.Eq(x => x.Id, eventId);
            var update = Builders<TowerEvent>.Update.Set(x => x.State, state);
            await context.TowerEvents.UpdateOneAsync(filter, update, new UpdateOptions { BypassDocumentValidation = true }, cancellationToken);
        }

        public async Task Update(TowerEvent towerEvent, CancellationToken cancellationToken)
        {
            var filter = Builders<TowerEvent>.Filter.Eq(x => x.Id, towerEvent.Id);
            await context.TowerEvents.ReplaceOneAsync(filter, towerEvent, new ReplaceOptions { BypassDocumentValidation = true }, cancellationToken);
        }

        public async Task Delete(Guid eventId, CancellationToken cancellationToken)
        {
            var filter = Builders<TowerEvent>.Filter.Eq(x => x.Id, eventId);
            await context.TowerEvents.DeleteOneAsync(filter, new DeleteOptions(), cancellationToken);
        }

        public async Task DeleteSpace(Guid spaceId, CancellationToken cancellationToken)
        {
            var filter = Builders<TowerEvent>.Filter.Where(x => x.Space != null && x.Space.Id == spaceId);
            var update = Builders<TowerEvent>.Update.Set(x => x.Space, null);
            await context.TowerEvents.UpdateManyAsync(filter, update, new UpdateOptions { BypassDocumentValidation = true }, cancellationToken);
        }

        public async Task DeleteUser(Guid userId, CancellationToken cancellationToken)
        {
            var filter = Builders<TowerEvent>.Filter.Where(x => x.SoldTickets.Any(t => t.UserId == userId));
            var towerEvents = await context.TowerEvents.Find(filter).ToListAsync(cancellationToken);
            foreach (var towerEvent in towerEvents)
            {
                towerEvent.SoldTickets = towerEvent.SoldTickets.Where(x => x.UserId != userId).ToArray();
                filter = Builders<TowerEvent>.Filter.Eq(x => x.Id, towerEvent.Id);
                var update = Builders<TowerEvent>.Update.Set(x => x.SoldTickets, towerEvent.SoldTickets);
                await context.TowerEvents.UpdateOneAsync(filter, update, new UpdateOptions { BypassDocumentValidation = true }, cancellationToken);
            }
        }

        public async Task<long> Count(AbstractFilter<TowerEvent>? filter, CancellationToken cancellationToken)
        {
            var filters = filter == null ? Builders<TowerEvent>.Filter.Empty : await filter.Filter(null, cancellationToken);
            var result = await context.TowerEvents.Find(filters).CountDocumentsAsync(cancellationToken);
            return result;
        }
    }
}
