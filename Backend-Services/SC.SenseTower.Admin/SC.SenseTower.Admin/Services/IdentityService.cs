using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Distributed;
using MongoDB.Bson;
using MongoDB.Driver;
using SC.SenseTower.Admin.Data.Contexts;
using SC.SenseTower.Admin.Data.Models.Identity;
using SC.SenseTower.Admin.Dto.Identity;
using SC.SenseTower.Common.Data;
using SC.SenseTower.Common.Exceptions;
using SC.SenseTower.Common.Extensions;
using SC.SenseTower.Common.Models;
using SC.SenseTower.Common.Services;
using System.Text.Json;

namespace SC.SenseTower.Admin.Services
{
    public class IdentityService : BaseDbService
    {
        private const string USERS_STATISTICS_KEY = "UsersStatistics";
        private const string USER_DETAILS_KEY = "UserDetails_";

        private new readonly IdentityDbContext context;

        private readonly IDistributedCache cache;
        private readonly UserManager<ApplicationUser> userManager;

        public IdentityService(
            ILogger<IdentityService> logger,
            IMapper mapper,
            IDistributedCache distributedCache,
            IdentityDbContext context,
            UserManager<ApplicationUser> userManager) : base(logger, mapper, context)
        {
            this.context = base.context as IdentityDbContext ?? null!;
            cache = distributedCache;
            this.userManager = userManager;
        }

        public async Task BanById(Guid userId, DateTime lockoutEnd, CancellationToken cancellationToken)
        {
            var user = await userManager.FindByIdAsync(userId.ToString());
            var identityResult = await userManager.SetLockoutEndDateAsync(user, lockoutEnd);
            if (!identityResult.Succeeded)
                throw new ScException($"Ошибка блокировки: {identityResult.GetMessages()}");
        }

        public async Task UnbanById(Guid userId, CancellationToken cancellationToken)
        {
            var user = await userManager.FindByIdAsync(userId.ToString());
            var identityResult = await userManager.SetLockoutEndDateAsync(user, null);
            if (!identityResult.Succeeded)
                throw new ScException($"Ошибка снятия блокировки: {identityResult.GetMessages()}");
        }

        public async Task<ApplicationUser?> Get(Guid userId, CancellationToken cancellationToken)
        {
            ApplicationUser? user;
            //var json = cache.GetString(USER_DETAILS_KEY + userId.ToString());
            //if (string.IsNullOrEmpty(json))
            //{
                var filter = Builders<ApplicationUser>.Filter.Eq(x => x.Id, userId);
                user = await context.Users.Find(filter).FirstOrDefaultAsync(cancellationToken);
            //    if (user != null)
            //        await cache.SetStringAsync(
            //            USER_DETAILS_KEY + userId.ToString(),
            //            JsonSerializer.Serialize(user),
            //            new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(1) }, // кэш для использования в валидации, поэтому время короткое
            //            cancellationToken);
            //}
            //else
            //    user = JsonSerializer.Deserialize<ApplicationUser>(json);
            return user;
        }

        public async Task<LookupItemDto<Guid>[]> GetRoleLookups(Guid[]? roleIds, CancellationToken cancellationToken)
        {
            var filter = roleIds != null ? Builders<ApplicationRole>.Filter.In(x => x.Id, roleIds) : new BsonDocument();
            var data = await context.Roles.Find(filter).ToListAsync(cancellationToken);
            return Mapper.Map<LookupItemDto<Guid>[]>(data.OrderBy(r => r.Name));
        }

        public async Task<IEnumerable<ApplicationUser>> GetUsers(QuerySorting[] sorting, FilterDefinition<ApplicationUser> filter, int start, int limit, CancellationToken cancellationToken)
        {
            var query = context.Users
                .Find(filter);
            if (sorting.Length > 0)
                query = query.Sort(sorting.ToDefinitions<ApplicationUser>());
            var result = await query
                .Skip(start)
                .Limit(limit)
                .ToListAsync(cancellationToken);
            return result.ToArray();
        }

        public async Task<long> Count(FilterDefinition<ApplicationUser> filter, CancellationToken cancellationToken)
        {
            return await context.Users.CountDocumentsAsync(filter, new CountOptions(), cancellationToken);
        }

        public async Task<IEnumerable<ApplicationUser>> GetByRole(string? role, bool equal, CancellationToken cancellationToken)
        {
            var roleFilter = Builders<ApplicationRole>.Filter.Eq(x => x.Name, role);
            var roleId = (await context.Roles.Find(roleFilter).FirstOrDefaultAsync(cancellationToken))?.Id;
            var userFilter = equal
                ? Builders<ApplicationUser>.Filter.Where(x => x.Roles.Any(r => r == roleId))
                : Builders<ApplicationUser>.Filter.Where(x => x.Roles.Any(r => r != roleId));
            return await context.Users.Find(userFilter).ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<ApplicationUser>> FindByName(string namePattern, CancellationToken cancellationToken = default)
        {
            namePattern = namePattern.ToUpperInvariant();
            var filter = Builders<ApplicationUser>.Filter.Where(r => r.NormalizedUserName.StartsWith(namePattern));
            return await context.Users.Find(filter).ToListAsync(cancellationToken);
        }

        public async Task ResetPassword(Guid userId, string token, string password, CancellationToken cancellationToken)
        {
            var user = await userManager.FindByIdAsync(userId.ToString());
            if (!await userManager.VerifyUserTokenAsync(user, userManager.Options.Tokens.PasswordResetTokenProvider, "ResetPassword", token))
                throw new ScException("Невалидный токен сброса пароля");

            var results = new List<string>();
            foreach (var validator in userManager.PasswordValidators)
            {
                var validationResult = await validator.ValidateAsync(userManager, user, password);
                if (!validationResult.Succeeded)
                    results.Add(validationResult.GetMessages());
            }
            if (results.Count > 0)
                throw new ScException(string.Join("\n", results));

            var resetResult = await userManager.ResetPasswordAsync(user, token, password);
            if (!resetResult.Succeeded)
                throw new ScException(resetResult.GetMessages());
        }

        public async Task<LookupItemDto<Guid>[]> GetUserLookups(IEnumerable<Guid?> userIds, CancellationToken cancellationToken)
        {
            var filter = userIds.Count() > 0 ? Builders<ApplicationUser>.Filter.In(x => x.Id, userIds) : new BsonDocument();
            var data = await context.Users.Find(filter).ToListAsync(cancellationToken);
            return Mapper.Map<LookupItemDto<Guid>[]>(data.OrderBy(r => r.UserName));
        }

        public async Task SetPassword(Guid userId, string currentPassword, string password, CancellationToken cancellationToken)
        {
            var user = await userManager.FindByIdAsync(userId.ToString());

            var results = new List<string>();
            foreach (var validator in userManager.PasswordValidators)
            {
                var validationResult = await validator.ValidateAsync(userManager, user, password);
                if (!validationResult.Succeeded)
                    results.Add(validationResult.GetMessages());
            }
            if (results.Count > 0)
                throw new ScException(string.Join("\n", results));

            var result = await userManager.ChangePasswordAsync(user, currentPassword, password);
            if (!result.Succeeded)
                throw new ScException(result.GetMessages());
        }

        public async Task<ApplicationUser?> GetUserByLoginOrEmail(string loginOrEmail, CancellationToken cancellationToken)
        {
            var user = await userManager.FindByNameAsync(loginOrEmail);
            if (user == null && loginOrEmail.Contains('@'))
            {
                user = await userManager.FindByEmailAsync(loginOrEmail);
            }
            return user;
        }

        public async Task<UserStatisticsDto> GetUserStatistics(CancellationToken cancellationToken)
        {
            var json = await cache.GetStringAsync(USERS_STATISTICS_KEY, cancellationToken);
            if (string.IsNullOrEmpty(json))
            {
                var result = new UserStatisticsDto();
                var now = DateTime.UtcNow;

                result.TotalCount = await context.Users.Find(new BsonDocument()).CountDocumentsAsync(cancellationToken);

                var monthStart = new DateTime(now.Year, now.Month, 1);
                var filter = Builders<ApplicationUser>.Filter.Where(x => x.CreatedOn >= monthStart && x.CreatedOn <= now);
                result.LastMonthCount = await context.Users.Find(filter).CountDocumentsAsync(cancellationToken);
                filter = Builders<ApplicationUser>.Filter.Where(x => x.CreatedOn >= monthStart.AddMonths(-1) && x.CreatedOn < monthStart);
                result.PrevMonthCount = await context.Users.Find(filter).CountDocumentsAsync(cancellationToken);

                var weekStart = now.AddDays(-(now.DayOfWeek == DayOfWeek.Sunday ? 6 : ((int)now.DayOfWeek - 1)));
                weekStart = new DateTime(weekStart.Year, weekStart.Month, weekStart.Day);
                filter = Builders<ApplicationUser>.Filter.Where(x => x.CreatedOn >= weekStart && x.CreatedOn <= now);
                result.LastWeekCount = await context.Users.Find(filter).CountDocumentsAsync(cancellationToken);
                filter = Builders<ApplicationUser>.Filter.Where(x => x.CreatedOn >= weekStart.AddDays(-7) && x.CreatedOn < weekStart);
                result.PrevWeekCount = await context.Users.Find(filter).CountDocumentsAsync(cancellationToken);

                var today = new DateTime(now.Year, now.Month, now.Day);
                filter = Builders<ApplicationUser>.Filter.Where(x => x.CreatedOn >= today && x.CreatedOn <= now);
                result.TodayCount = await context.Users.Find(filter).CountDocumentsAsync(cancellationToken);
                filter = Builders<ApplicationUser>.Filter.Where(x => x.CreatedOn >= today.AddDays(-1) && x.CreatedOn < today);
                result.YesterdayCount = await context.Users.Find(filter).CountDocumentsAsync(cancellationToken);

                json = JsonSerializer.Serialize(result);
                await cache.SetStringAsync(USERS_STATISTICS_KEY, json, new DistributedCacheEntryOptions { SlidingExpiration = TimeSpan.FromMinutes(10) }, cancellationToken);

                return result;
            }

            return JsonSerializer.Deserialize<UserStatisticsDto>(json) ?? new UserStatisticsDto();
        }
    }
}
