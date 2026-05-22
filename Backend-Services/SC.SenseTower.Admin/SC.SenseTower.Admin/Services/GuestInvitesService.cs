using AutoMapper;
using MongoDB.Driver;
using SC.SenseTower.Admin.Data.Contexts;
using SC.SenseTower.Admin.Data.Models.Accounts;
using SC.SenseTower.Common.Data;
using SC.SenseTower.Common.Exceptions;
using SC.SenseTower.Common.Extensions;
using SC.SenseTower.Common.Services;

namespace SC.SenseTower.Admin.Services
{
    public class GuestInvitesService : BaseDbService
    {
        private new readonly AccountsDbContext context;

        public GuestInvitesService(
            ILogger<GuestInvitesService> logger,
            IMapper mapper,
            AccountsDbContext context) : base(logger, mapper, context)
        {
            this.context = base.context as AccountsDbContext ?? null!;
        }

        public async Task<long> Count(FilterDefinition<Ticket> filter, CancellationToken cancellationToken)
        {
            return await context.Tickets.CountDocumentsAsync(filter, new CountOptions(), cancellationToken);
        }

        public async Task Recall(string id, string? reason, CancellationToken cancellationToken)
        {
            var filter = Builders<Ticket>.Filter.Eq(x => x.Id, id);
            var update = Builders<Ticket>.Update.Set(x => x.RecallInfo, new RecallInfo { Date = DateTime.UtcNow, IsRecalled = true, RecallReason = reason });
            var updateResult = await context.Tickets.UpdateOneAsync(filter, update, new UpdateOptions { BypassDocumentValidation = true }, cancellationToken);
            if (updateResult.ModifiedCount != 1)
                throw new ScException($"Ошибка при отзыве билета {id}.");
        }

        public async Task<IEnumerable<Ticket>> GetUserTickets(Guid userId, CancellationToken cancellationToken)
        {
            var filter = Builders<Ticket>.Filter.Eq(x => x.IssuerId, userId);
            var result = await context.Tickets.Find(filter).ToListAsync(cancellationToken);
            return result.OrderByDescending(r => r.CreatedAt);
        }

        public async Task<string[]> CreateBatch(Guid userId, int quantity, CancellationToken cancellationToken)
        {
            var tickets = Enumerable.Range(0, quantity)
                .Select(x => new Ticket
                {
                    Id = Guid.NewGuid().ToString().Replace("-", ""),
                    IssuerId = userId,
                    CreatedAt = DateTime.UtcNow
                })
                .ToArray();
            await context.Tickets.InsertManyAsync(tickets, new InsertManyOptions { BypassDocumentValidation = true }, cancellationToken);
            return tickets.Select(x => x.Id).ToArray();
        }

        public async Task<Ticket> Get(string id, CancellationToken cancellationToken)
        {
            var filter = Builders<Ticket>.Filter.Eq(x => x.Id, id);
            var result = await context.Tickets.Find(filter).FirstOrDefaultAsync(cancellationToken);
            return result;
        }

        public async Task<Ticket[]> Get(QuerySorting[] sorting, FilterDefinition<Ticket> filter, int start, int limit, CancellationToken cancellationToken)
        {
            var query = context.Tickets
                .Find(filter);
            if (sorting.Length > 0)
                query = query.Sort(sorting.ToDefinitions<Ticket>());
            var result = await query
                .Skip(start)
                .Limit(limit)
                .ToListAsync(cancellationToken);
            return result.ToArray();
        }
    }
}
