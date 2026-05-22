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
    public class InvitesService : BaseDbService
    {
        private new readonly AccountsDbContext context;

        public InvitesService(
            ILogger<InvitesService> logger,
            IMapper mapper,
            AccountsDbContext context) : base(logger, mapper, context)
        {
            this.context = base.context as AccountsDbContext ?? null!;
        }

        public async Task<long> Count(FilterDefinition<Invite> filter, CancellationToken cancellationToken)
        {
            return await context.Invites.CountDocumentsAsync(filter, new CountOptions(), cancellationToken);
        }

        public async Task Recall(string id, string? reason, CancellationToken cancellationToken)
        {
            var filter = Builders<Invite>.Filter.Eq(x => x.Id, id);
            var update = Builders<Invite>.Update.Set(x => x.RecallInfo, new RecallInfo { Date = DateTime.UtcNow, IsRecalled = true, RecallReason = reason });
            var updateResult = await context.Invites.UpdateOneAsync(filter, update, new UpdateOptions { BypassDocumentValidation = true }, cancellationToken);
            if (updateResult.ModifiedCount != 1)
                throw new ScException($"Ошибка при отзыве приглашения {id}.");
        }

        public async Task<IEnumerable<Invite>> GetUserInvites(Guid userId, CancellationToken cancellationToken)
        {
            var filter = Builders<Invite>.Filter.Eq(x => x.IssuerId, userId);
            var result = await context.Invites.Find(filter).ToListAsync(cancellationToken);
            return result.OrderByDescending(r => r.CreatedAt);
        }

        public async Task<string[]> CreateBatch(Guid authorId, Guid userId, int quantity, CancellationToken cancellationToken)
        {
            var invites = Enumerable.Range(0, quantity)
                .Select(x => new Invite
                {
                    AuthorId = authorId,
                    Id = Guid.NewGuid().ToString().Replace("-", ""),
                    IssuerId = userId,
                    CreatedAt = DateTime.UtcNow
                })
                .ToArray();
            await context.Invites.InsertManyAsync(invites, new InsertManyOptions { BypassDocumentValidation = true }, cancellationToken);
            return invites.Select(x => x.Id).ToArray();
        }

        public async Task<Invite> Get(string inviteId, CancellationToken cancellationToken)
        {
            var filter = Builders<Invite>.Filter.Eq(x => x.Id, inviteId);
            var result = await context.Invites.Find(filter).FirstOrDefaultAsync(cancellationToken);
            return result;
        }

        public async Task<Invite[]> GetInvites(QuerySorting[] sorting, FilterDefinition<Invite> filter, int start, int limit, CancellationToken cancellationToken)
        {
            var query = context.Invites
                .Find(filter);
            if (sorting.Length > 0)
                query = query.Sort(sorting.ToDefinitions<Invite>());
            var result = await query
                .Skip(start)
                .Limit(limit)
                .ToListAsync(cancellationToken);
            return result.ToArray();
        }
    }
}
