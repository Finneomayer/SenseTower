using AutoMapper;
using MongoDB.Driver;
using SC.SenseTower.Accounts.Data;
using SC.SenseTower.Accounts.Data.Models;
using SC.SenseTower.Common.Exceptions;
using SC.SenseTower.Common.Services;

namespace SC.SenseTower.Accounts.Services
{
    public class InvitesService : BaseDbService
    {
        private new AccountsDbContext context => base.context as AccountsDbContext ?? null!;

        public InvitesService(
            ILogger<InvitesService> logger,
            IMapper mapper,
            AccountsDbContext context) : base(logger, mapper, context)
        {
        }

        /// <summary>
        /// Отзыв приглашения.
        /// </summary>
        /// <param name="inviteId">Идентификатор (код) приглашения.</param>
        /// <param name="reason">Причина отзыва.</param>
        /// <param name="cancellationToken">Токен прерывания выполнения.</param>
        /// <returns></returns>
        /// <exception cref="ScException"></exception>
        public async Task<bool> Recall(string? inviteId, string? reason, CancellationToken cancellationToken)
        {
            var filter = Builders<Invite>.Filter.Eq(r => r.Id, inviteId);
            var update = Builders<Invite>.Update
                .Set(r => r.RecallInfo.IsRecalled, true)
                .Set(r => r.RecallInfo.Date, DateTime.UtcNow)
                .Set(r => r.RecallInfo.RecallReason, reason);
            var updateResult = await context.Invites.UpdateOneAsync(filter, update, new UpdateOptions { BypassDocumentValidation = true }, cancellationToken);
            if (updateResult.ModifiedCount == 0)
                throw new ScException("Ошибка отзыва приглашения.");
            return true;
        }

        /// <summary>
        /// Удаление инвайтов, выданных пользователю при подтверждении почты.
        /// </summary>
        /// <param name="userId">Идентификатор пользователя.</param>
        /// <param name="cancellationToken">Токен прерывания выполнения.</param>
        /// <returns>Признак успешного завершения операции.</returns>
        public async Task<bool> DeleteByUser(Guid userId, CancellationToken cancellationToken)
        {
            var filter = Builders<Invite>.Filter.Eq(x => x.IssuerId, userId);
            await context.Invites.DeleteManyAsync(filter, cancellationToken);
            return true;
        }

        /// <summary>
        /// Подсчет количества инвайтов пользователя.
        /// </summary>
        /// <param name="userId">Идентификатор пользователя.</param>
        /// <param name="cancellationToken">Токен прерывания выполнения.</param>
        /// <returns>Число инвайтов, выданных пользователю при подтверждении почты.</returns>
        public async Task<int> CountByUser(Guid userId, CancellationToken cancellationToken)
        {
            var filter = Builders<Invite>.Filter.Eq(x => x.IssuerId, userId);
            return (int)await context.Invites.Find(filter).CountDocumentsAsync(cancellationToken);
        }

        /// <summary>
        /// Создание пакета инвайтов для пользователя при подтверждении email.
        /// </summary>
        /// <param name="authorId">Идентификатор пользователя, создавшего инвайт.</param>
        /// <param name="userId">Идентификатор пользователя-владельца инвайта.</param>
        /// <param name="quantity">Количество инвайтов в пакете.</param>
        /// <param name="cancellationToken">Токен прерывания выполнения.</param>
        /// <returns></returns>
        public async Task<string[]> CreateUserInvites(Guid authorId, Guid userId, int quantity, CancellationToken cancellationToken)
        {
            var invites = Enumerable.Range(1, quantity)
                .Select(_ => new Invite
                {
                    AuthorId = authorId,
                    Id = Guid.NewGuid().ToString().Replace("-", ""),
                    IssuerId = userId
                })
                .ToArray();
            await context.Invites.InsertManyAsync(invites, new InsertManyOptions { BypassDocumentValidation = true }, cancellationToken);
            return invites.Select(r => r.Id).ToArray();
        }

        /// <summary>
        /// Получение инвайта по его идентификатору.
        /// </summary>
        /// <param name="inviteId">Идентификатор инвайта.</param>
        /// <param name="cancellationToken">Токен прерывания выполнения.</param>
        /// <returns>Сущность инвайта</returns>
        public async Task<Invite?> Get(string inviteId, CancellationToken cancellationToken)
        {
            var filter = Builders<Invite>.Filter.Eq(r => r.Id, inviteId);
            return await context.Invites.Find(filter).FirstOrDefaultAsync(cancellationToken);
        }

        /// <summary>
        /// Пометка инвайта как использованного указанным пользователем.
        /// </summary>
        /// <param name="inviteId">Идентификатор (код) инвайта.</param>
        /// <param name="userId">Идентификатор пользователя.</param>
        /// <param name="cancellationToken">Токен прерывания выполнения.</param>
        /// <returns></returns>
        /// <exception cref="ScException">Исключение, обрабатываемое ПО промежуточного слоя.</exception>
        public async Task MarkAsUsed(string inviteId, Guid userId, CancellationToken cancellationToken)
        {
            var filter = Builders<Invite>.Filter.Eq(r => r.Id, inviteId);
            var update = Builders<Invite>.Update
                .Set(r => r.UsingInfo.UserId, userId)
                .Set(r => r.UsingInfo.Date, DateTime.UtcNow);
            var updateResult = await context.Invites.UpdateOneAsync(filter, update, new UpdateOptions { BypassDocumentValidation = true }, cancellationToken);
            if (updateResult.ModifiedCount == 0)
                throw new ScException("Ошибка установки признака использованного приглашения.");
        }

        /// <summary>
        /// Получение списка инвайтов пользователя.
        /// </summary>
        /// <param name="userId">Идентификатор пользователя.</param>
        /// <param name="cancellationToken">Токен прерывания выполнения.</param>
        /// <returns>Массив инвайтов пользователя.</returns>
        public async Task<Invite[]> GetUserInvites(Guid userId, CancellationToken cancellationToken)
        {
            var filter = Builders<Invite>.Filter.Eq(r => r.IssuerId, userId);
            var result = await context.Invites.Find(filter).ToListAsync(cancellationToken);
            return result.ToArray();
        }

        /// <summary>
        /// Поиск инвайта, по которому пользователь был зарегистрирован.
        /// </summary>
        /// <param name="userId">Идентификатор пользователя.</param>
        /// <param name="cancellationToken">Токен прерывания выполнения.</param>
        /// <returns>Инвайт.</returns>
        public async Task<Invite?> GetByUser(Guid userId, CancellationToken cancellationToken)
        {
            var filter = Builders<Invite>.Filter.Eq(x => x.UsingInfo.UserId, userId);
            var result = await context.Invites.Find(filter).FirstOrDefaultAsync(cancellationToken);
            return result;
        }
    }
}
