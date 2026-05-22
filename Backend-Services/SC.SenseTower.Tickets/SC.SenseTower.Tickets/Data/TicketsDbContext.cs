using Microsoft.Extensions.Options;
using MongoDB.Driver;
using SC.SenseTower.Common.Data;
using SC.SenseTower.Common.Models;
using SC.SenseTower.Tickets.Constants;
using SC.SenseTower.Tickets.Data.Models;

namespace SC.SenseTower.Tickets.Data
{
    public class TicketsDbContext : BaseDbContext
    {
        private IMongoCollection<Ticket>? tickets;
        public IMongoCollection<Ticket> Tickets
        {
            get
            {
                tickets ??= GetDbCollection<Ticket>(DbConstants.COLLECTION_TICKETS);
                return tickets;
            }
        }

        public TicketsDbContext(IOptions<MongoDbConfig> options) : base(options) { }
    }
}
