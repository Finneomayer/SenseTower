using MongoDB.Driver;
using SC.SenseTower.Admin.Data.Models.Accounts;
using SC.SenseTower.Admin.Services;
using SC.SenseTower.Common.Data;
using SC.SenseTower.Common.Services;

namespace SC.SenseTower.Admin.Cqrs.InvitesPage
{
    public class InvitesPageFilter : AbstractFilter<Invite>
    {
        public string? AuthorName { get; set; }

        public string? IssuerName { get; set; }

        public string? StatusName { get; set; }

        public string? UserName { get; set; }

        public override async Task<FilterDefinition<Invite>> Filter(BaseDbService? service, CancellationToken cancellationToken)
        {
            var filter = StatusName switch
            {
                "Published" => Builders<Invite>.Filter.Where(r => (r.UsingInfo == null || r.UsingInfo.Date == null) && (r.RecallInfo == null || !r.RecallInfo.IsRecalled)),
                "Used" => Builders<Invite>.Filter.Where(r => r.UsingInfo != null && r.UsingInfo.Date != null),
                "Recalled" => Builders<Invite>.Filter.Where(r => r.RecallInfo != null && r.RecallInfo.IsRecalled),
                _ => Builders<Invite>.Filter.Empty
            };

            var identityService = service as IdentityService;
            if (identityService != null)
            {
                if (!string.IsNullOrWhiteSpace(AuthorName))
                {
                    var userIds = (await identityService.FindByName(AuthorName, cancellationToken)).Select(r => r.Id).ToArray();
                    filter = Builders<Invite>.Filter.And(filter, Builders<Invite>.Filter.Where(x => x.AuthorId != null && userIds.Contains(x.AuthorId.Value)));
                }
                if (!string.IsNullOrWhiteSpace(IssuerName))
                {
                    var userIds = (await identityService.FindByName(IssuerName, cancellationToken)).Select(r => r.Id).ToArray();
                    filter = Builders<Invite>.Filter.And(filter, Builders<Invite>.Filter.Where(x => x.IssuerId != null && userIds.Contains(x.IssuerId.Value)));
                }
                if (!string.IsNullOrWhiteSpace(UserName))
                {
                    var userIds = (await identityService.FindByName(UserName, cancellationToken)).Select(r => r.Id).ToArray();
                    filter = Builders<Invite>.Filter.And(filter, Builders<Invite>.Filter.Where(x => x.UsingInfo != null && x.UsingInfo.UserId != null && userIds.Contains(x.UsingInfo.UserId.Value)));
                }
            }

            return filter;
        }
    }
}