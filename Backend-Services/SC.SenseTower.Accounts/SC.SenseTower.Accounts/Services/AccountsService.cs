using AutoMapper;
using MongoDB.Driver;
using SC.SenseTower.Accounts.Data;
using SC.SenseTower.Accounts.Data.Models;
using SC.SenseTower.Common.Exceptions;
using SC.SenseTower.Common.Services;

namespace SC.SenseTower.Accounts.Services
{
    public class AccountsService : BaseDbService
    {
        private new AccountsDbContext context => base.context as AccountsDbContext ?? null!;

        public AccountsService(
            ILogger<AccountsService> logger,
            IMapper mapper,
            AccountsDbContext context) : base(logger, mapper, context)
        {
        }

        /// <summary>
        /// Создание аккаунта пользователя.
        /// </summary>
        /// <param name="userId">Идентификатор пользователя.</param>
        /// <param name="referrerId">Идентификатор пользователя, выдавшего приглашение.</param>
        /// <param name="cancellationToken">Токен прерывания выполнения.</param>
        /// <returns>Идентификатор аккаунта (здесь всегда есть и равен идентификатору пользователя, сделано nullable для соответствия общему правилу создания сущности).</returns>
        public async Task<Guid?> Create(Guid userId, Guid? referrerId, CancellationToken cancellationToken)
        {
            try
            {
                var account = new Account { Id = userId, ReferrerId = referrerId };
                await context.Accounts.InsertOneAsync(account, new InsertOneOptions { BypassDocumentValidation = true }, cancellationToken);
                return userId;
            }
            catch(Exception ex)
            {
                Logger.LogError($"Ошибка создания аккаунта: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Сброс информации о восстановлении пароля.
        /// </summary>
        /// <param name="userId">Идентификатор пользователя.</param>
        /// <param name="cancellationToken">Токен прерывания выполнения.</param>
        /// <returns></returns>
        public async Task ClearPasswordResetInfo(Guid userId, CancellationToken cancellationToken)
        {
            var info = new PasswordResetInfo();
            var filter = Builders<Account>.Filter.Eq(x => x.Id, userId);
            var update = Builders<Account>.Update.Set(x => x.PasswordResetInfo, info);
            var result = await context.Accounts.UpdateOneAsync(filter, update, new UpdateOptions { BypassDocumentValidation = true }, cancellationToken);
            if (result.ModifiedCount == 0)
                throw new ScException("Данные аккаунта не обновлены.");
        }

        /// <summary>
        /// Сохранение токена восстановления пароля.
        /// </summary>
        /// <param name="userId">Идентификатор пользователя, восстанавливающего пароль.</param>
        /// <param name="token">Токен восстановления пароля.</param>
        /// <param name="cancellationToken">Токен прерывания выполнения.</param>
        /// <returns></returns>
        public async Task SaveResetPasswordToken(Guid userId, string token, CancellationToken cancellationToken)
        {
            var filter = Builders<Account>.Filter.Eq(x => x.Id, userId);
            var passwordResetInfo = new PasswordResetInfo
            {
                Token = token,
                ExpiredAt = DateTime.UtcNow.AddHours(1)
            };
            var update = Builders<Account>.Update.Set(x => x.PasswordResetInfo, passwordResetInfo);
            var updateResult = await context.Accounts.UpdateOneAsync(filter, update, new UpdateOptions { BypassDocumentValidation = true }, cancellationToken);
            if (updateResult.ModifiedCount == 0)
                throw new ScException("Ошибка сохранения информации о восстановлении пароля.");
        }

        /// <summary>
        /// Удаление аккаунта пользователя.
        /// </summary>
        /// <param name="userId">Идентификатор пользователя.</param>
        /// <param name="cancellationToken">Токен прерывания выполнения.</param>
        /// <returns>Признак успешного выполнения операции.</returns>
        public async Task<bool> Delete(Guid userId, CancellationToken cancellationToken)
        {
            var filter = Builders<Account>.Filter.Eq(x => x.Id, userId);
            await context.Accounts.DeleteOneAsync(filter, new DeleteOptions(), cancellationToken);
            return true;
        }

        /// <summary>
        /// Получение аккаунта пользователя по идентификатору.
        /// </summary>
        /// <param name="userId">Идентификатор пользователя.</param>
        /// <param name="cancellationToken">Токен прерывания выполнения.</param>
        /// <returns>Сущность аккаунта пользователя.</returns>
        public async Task<Account> Get(Guid userId, CancellationToken cancellationToken)
        {
            var filter = Builders<Account>.Filter.Eq(x => x.Id, userId);
            return await context.Accounts.Find(filter).FirstOrDefaultAsync(cancellationToken);
        }

        /// <summary>
        /// Установка/сброс аватара пользователя.
        /// </summary>
        /// <param name="userId">Идентификатор пользователя.</param>
        /// <param name="avatarNumber">Индекс аватара в коллекции.</param>
        /// <param name="cancellationToken">Токен прерывания выполнения.</param>
        /// <returns></returns>
        public async Task SetAvatar(Guid userId, int? avatarNumber, CancellationToken cancellationToken)
        {
            var filter = Builders<Account>.Filter.Eq(x => x.Id, userId);
            var update = Builders<Account>.Update.Set(x => x.AvatarNumber, avatarNumber);
            await context.Accounts.UpdateOneAsync(filter, update, new UpdateOptions { BypassDocumentValidation = true }, cancellationToken);
        }

        public async Task<bool> Exists(Guid userId, CancellationToken cancellationToken)
        {
            var filter = Builders<Account>.Filter.Eq(x => x.Id, userId);
            return await context.Accounts.Find(filter).AnyAsync(cancellationToken);
        }
    }
}
