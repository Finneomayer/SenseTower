using AutoMapper;
using MongoDB.Driver;
using SC.SenseTower.Admin.Data.Contexts;
using SC.SenseTower.Admin.Data.Models.Accounts;
using SC.SenseTower.Common.Services;

namespace SC.SenseTower.Admin.Services
{
    public class WalletsService : BaseDbService
    {
        private new readonly AccountsDbContext context;

        public WalletsService(
            ILogger<WalletsService> logger,
            IMapper mapper,
            AccountsDbContext context) : base(logger, mapper, context)
        {
            this.context = base.context as AccountsDbContext ?? null!;
        }

        public async Task<IEnumerable<Wallet>> GetUserWallets(Guid userId, CancellationToken cancellationToken)
        {
            var filter = Builders<Wallet>.Filter.Eq(x => x.OwnerId, userId);
            var wallets = await context.Wallets.Find(filter).ToListAsync(cancellationToken);
            return wallets.OrderBy(r => r.Id);
        }
    }
}
