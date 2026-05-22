using AutoMapper;
using MongoDB.Driver;
using SC.SenseTower.Common.Services;
using SC.SenseTower.Tickets.Data;
using SC.SenseTower.Tickets.Data.Models;

namespace SC.SenseTower.Tickets.Services
{
    public class TicketsService : BaseDbService
    {
        private new readonly TicketsDbContext context;

        public TicketsService(
            ILogger<TicketsService> logger,
            IMapper mapper,
            TicketsDbContext context) : base(logger, mapper, context)
        {
            this.context = base.context as TicketsDbContext ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IEnumerable<Ticket>> GetByEventId(Guid eventId, CancellationToken cancellationToken)
        {
            var filter = Builders<Ticket>.Filter.Eq(x => x.TowerEvent.Id, eventId);
            var result = await context.Tickets.Find(filter).ToListAsync(cancellationToken);
            return result;
        }

        public async Task Create(Ticket ticket, int? quantity, CancellationToken cancellationToken)
        {
            var tickets = Enumerable.Range(0, quantity ?? 1)
                .Select(x =>
                {
                    var item = Mapper.Map<Ticket>(ticket);
                    item.Id = Guid.NewGuid();
                    return item;
                })
                .ToArray();
            await context.Tickets.InsertManyAsync(tickets, new InsertManyOptions { BypassDocumentValidation = true }, cancellationToken);
        }

        public async Task<Guid?> Buy(Guid eventId, Guid userId, CancellationToken cancellationToken)
        {
            var filter = Builders<Ticket>.Filter.Eq(x => x.TowerEvent.Id, eventId);
            var tickets = await context.Tickets.Find(filter).ToListAsync(cancellationToken);
            filter = Builders<Ticket>.Filter.And(filter, Builders<Ticket>.Filter.Where(x => x.UserId == null));
            var ticket = await context.Tickets.Find(filter).FirstOrDefaultAsync(cancellationToken);
            if (ticket != null)
            {
                filter = Builders<Ticket>.Filter.Eq(x => x.Id, ticket.Id);
                var update = Builders<Ticket>.Update.Set(x => x.UserId, userId);
                await context.Tickets.UpdateOneAsync(filter, update, new UpdateOptions { BypassDocumentValidation = true }, cancellationToken);
            }
            return ticket?.Id;
        }

        public async Task<int> CountTotal(Guid eventId, CancellationToken cancellationToken)
        {
            var filter = Builders<Ticket>.Filter.Eq(x => x.TowerEvent.Id, eventId);
            var result = await context.Tickets.Find(filter).CountDocumentsAsync(cancellationToken);
            return (int)result;
        }

        public async Task ClearUser(Guid userId, CancellationToken cancellationToken)
        {
            var filter = Builders<Ticket>.Filter.Eq(x => x.UserId, userId);
            var update = Builders<Ticket>.Update.Set(x => x.UserId, null);
            await context.Tickets.UpdateManyAsync(filter, update, new UpdateOptions { BypassDocumentValidation = true }, cancellationToken);
        }

        public async Task UpdateTowerEvent(TowerEvent towerEvent, CancellationToken cancellationToken)
        {
            var filter = Builders<Ticket>.Filter.Eq(x => x.TowerEvent.Id, towerEvent.Id);
            var update = Builders<Ticket>.Update.Set(x => x.TowerEvent, towerEvent);
            await context.Tickets.UpdateManyAsync(filter, update, new UpdateOptions { BypassDocumentValidation = true }, cancellationToken);
        }

        public async Task DeleteTowerEvent(Guid eventId, CancellationToken cancellationToken)
        {
            var filter = Builders<Ticket>.Filter.Eq(x => x.TowerEvent.Id, eventId);
            await context.Tickets.DeleteManyAsync(filter, new DeleteOptions(), cancellationToken);
        }

        public async Task<IEnumerable<Ticket>> GetSold(Guid eventId, CancellationToken cancellationToken)
        {
            var filter = Builders<Ticket>.Filter.Eq(x => x.TowerEvent.Id, eventId);
            filter = Builders<Ticket>.Filter.And(filter, Builders<Ticket>.Filter.Where(x => x.UserId != null && x.UserId != default));
            var result = await context.Tickets.Find(filter).ToListAsync(cancellationToken);
            return result ?? new List<Ticket>();
        }
    }
}
