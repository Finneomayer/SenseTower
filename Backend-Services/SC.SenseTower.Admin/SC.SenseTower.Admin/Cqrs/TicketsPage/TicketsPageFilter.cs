using MongoDB.Driver;
using SC.SenseTower.Admin.Data.Models.Accounts;
using SC.SenseTower.Admin.Services;
using SC.SenseTower.Common.Data;
using SC.SenseTower.Common.Services;

namespace SC.SenseTower.Admin.Cqrs.TicketsPage
{
    public class TicketsPageFilter : AbstractFilter<Ticket>
    {
        public string? IssuerName { get; set; }

        public string? StatusName { get; set; }

        public string? UserName { get; set; }

        public override async Task<FilterDefinition<Ticket>> Filter(BaseDbService? service, CancellationToken cancellationToken)
        {
            var filter = StatusName switch
            {
                "Published" => Builders<Ticket>.Filter.Where(r => (r.UsingInfo == null || r.UsingInfo.Date == null) && (r.RecallInfo == null || !r.RecallInfo.IsRecalled)),
                "Used" => Builders<Ticket>.Filter.Where(r => r.UsingInfo != null && r.UsingInfo.Date != null),
                "Recalled" => Builders<Ticket>.Filter.Where(r => r.RecallInfo != null && r.RecallInfo.IsRecalled),
                _ => Builders<Ticket>.Filter.Empty
            };

            var identityService = service as IdentityService;
            if (identityService != null)
            {
                if (!string.IsNullOrWhiteSpace(IssuerName))
                {
                    var userIds = (await identityService.FindByName(IssuerName, cancellationToken)).Select(r => r.Id).ToArray();
                    filter = Builders<Ticket>.Filter.And(filter, Builders<Ticket>.Filter.Where(x => x.IssuerId != null && userIds.Contains(x.IssuerId.Value)));
                }
                if (!string.IsNullOrWhiteSpace(UserName))
                {
                    var userIds = (await identityService.FindByName(UserName, cancellationToken)).Select(r => r.Id).ToArray();
                    filter = Builders<Ticket>.Filter.And(filter, Builders<Ticket>.Filter.Where(x => x.UsingInfo != null && x.UsingInfo.UserId != null && userIds.Contains(x.UsingInfo.UserId.Value)));
                }
            }

            return filter;
        }
    }
}