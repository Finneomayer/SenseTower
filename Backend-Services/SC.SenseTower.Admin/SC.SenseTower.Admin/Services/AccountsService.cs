using AutoMapper;
using MongoDB.Bson;
using MongoDB.Driver;
using SC.SenseTower.Admin.Data.Contexts;
using SC.SenseTower.Admin.Data.Models.Accounts;
using SC.SenseTower.Common.Services;

namespace SC.SenseTower.Admin.Services
{
    public class AccountsService : BaseDbService
    {
        private new readonly AccountsDbContext context;

        public AccountsService(ILogger<AccountsService> logger, IMapper mapper, AccountsDbContext context) : base(logger, mapper, context)
        {
            this.context = base.context as AccountsDbContext ?? null!;
        }

        public async Task<IEnumerable<Account>> GetAll(CancellationToken cancellationToken)
        {
            return await context.Accounts.Find(new BsonDocument()).ToListAsync(cancellationToken);
        }

        public async Task<Guid> Create(Guid userId, CancellationToken cancellationToken)
        {
            var account = new Account { Id = userId };
            await context.Accounts.InsertOneAsync(account, new InsertOneOptions { BypassDocumentValidation = true }, cancellationToken);
            return userId;
        }
    }
}
