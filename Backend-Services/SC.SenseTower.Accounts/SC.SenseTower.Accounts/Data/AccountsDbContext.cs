using Microsoft.Extensions.Options;
using MongoDB.Driver;
using SC.SenseTower.Accounts.Constants;
using SC.SenseTower.Accounts.Data.Models;
using SC.SenseTower.Common.Data;
using SC.SenseTower.Common.Models;

namespace SC.SenseTower.Accounts.Data
{
    public class AccountsDbContext : BaseDbContext
    {
        private IMongoCollection<Account>? accounts = null;
        public IMongoCollection<Account> Accounts => accounts ??= GetDbCollection<Account>(DbConstants.COLLECTION_ACCOUNTS);

        private IMongoCollection<Wallet>? wallets = null;
        public IMongoCollection<Wallet> Wallets => wallets ??= GetDbCollection<Wallet>(DbConstants.COLLECTION_WALLETS);

        private IMongoCollection<Invite>? invites = null;
        public IMongoCollection<Invite> Invites => invites ??= GetDbCollection<Invite>(DbConstants.COLLECTION_INVITES);

        private IMongoCollection<Ticket>? tickets = null;
        public IMongoCollection<Ticket> Tickets => tickets ??= GetDbCollection<Ticket>(DbConstants.COLLECTION_TICKETS);

        public AccountsDbContext(IOptions<MongoDbConfig> options) : base(options)
        {
        }
    }
}
