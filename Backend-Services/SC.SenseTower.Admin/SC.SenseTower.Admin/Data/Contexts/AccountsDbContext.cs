using Microsoft.Extensions.Options;
using MongoDB.Driver;
using SC.SenseTower.Admin.Constants;
using SC.SenseTower.Admin.Data.Models.Accounts;
using SC.SenseTower.Admin.Settings;
using SC.SenseTower.Common.Data;

namespace SC.SenseTower.Admin.Data.Contexts
{
    public class AccountsDbContext : BaseDbContext
    {
        private IMongoCollection<Account>? accounts;
        public IMongoCollection<Account> Accounts => accounts ??= GetDbCollection<Account>(DbConstants.COLLECTION_ACCOUNTS);

        private IMongoCollection<Invite>? invites;
        public IMongoCollection<Invite> Invites => invites ??= GetDbCollection<Invite>(DbConstants.COLLECTION_INVITES);

        private IMongoCollection<Ticket>? tickets;
        public IMongoCollection<Ticket> Tickets => tickets ??= GetDbCollection<Ticket>(DbConstants.COLLECTION_TICKETS);

        private IMongoCollection<Wallet>? wallets;
        public IMongoCollection<Wallet> Wallets => wallets ??= GetDbCollection<Wallet>(DbConstants.COLLECTION_WALLETS);

        public AccountsDbContext(IOptions<AccountsDbConfig> options) : base(options) { }
    }
}
