using AutoMapper;
using MongoDB.Bson;
using MongoDB.Driver;
using SC.SenseTower.Accounts.Data;
using SC.SenseTower.Accounts.Data.Models;
using SC.SenseTower.Accounts.Dto.Wallets;
using SC.SenseTower.Common.Exceptions;
using SC.SenseTower.Common.Services;

namespace SC.SenseTower.Accounts.Services
{
    /// <summary>
    /// Сервис управления кошельками пользователя.
    /// </summary>
    public class WalletsService : BaseDbService
    {
        private new AccountsDbContext context => base.context as AccountsDbContext ?? null!;

        public WalletsService(
            ILogger<WalletsService> logger,
            IMapper mapper,
            AccountsDbContext context) : base(logger, mapper, context)
        {
        }

        /// <summary>
        /// Подтверждение кошелька.
        /// </summary>
        /// <param name="walletId">Идентификатор кошелька.</param>
        /// <param name="cancellationToken">Токен прерывания выполнения.</param>
        /// <returns>Признак успешного выполнения операции.</returns>
        /// <exception cref="ScException"></exception>
        public async Task<bool> Confirm(string? walletId, CancellationToken cancellationToken)
        {
            var filter = Builders<Wallet>.Filter.Eq(x => x.Id, walletId);
            var update = Builders<Wallet>.Update.Set(x => x.IsConfirmed, true).Set(x => x.IsActive, true);
            var result = await context.Wallets.UpdateOneAsync(filter, update, null, cancellationToken).ConfigureAwait(false);
            if (!result.IsAcknowledged)
                throw new ScException("Результат выполнения операции не подтверждён.");
            return true;
        }

        /// <summary>
        /// Получение кошелька по его идентификатору.
        /// </summary>
        /// <param name="walletId">Идентификатор кошелька пользователя.</param>
        /// <param name="cancellationToken">Токен прерывания выполнения.</param>
        /// <returns>Сущность кошелька пользователя.</returns>
        public async Task<Wallet> GetWallet(string walletId, CancellationToken cancellationToken)
        {
            return await context.Wallets.Find(r => r.Id == walletId).FirstOrDefaultAsync(cancellationToken).ConfigureAwait(false);
        }

        public async Task<bool> DeleteByUser(Guid? userId, CancellationToken cancellationToken)
        {
            var filter = Builders<Wallet>.Filter.Eq(x => x.OwnerId, userId);
            var result = await context.Wallets.DeleteManyAsync(filter, cancellationToken).ConfigureAwait(false);
            if (!result.IsAcknowledged)
                throw new ScException($"Удаление кошельков пользователя {userId} не подтверждено.");
            return true;
        }

        /// <summary>
        /// Добавление пользовательского кошелька.
        /// </summary>
        /// <param name="wallet">Сущность нового кошелька пользоввателя.</param>
        /// <param name="cancellationToken">Токен прерывания выполнения.</param>
        /// <returns>Идентификатор нового кошелька.</returns>
        public async Task<string> AddWallet(Wallet wallet, CancellationToken cancellationToken)
        {
            await context.Wallets.InsertOneAsync(wallet, null, cancellationToken).ConfigureAwait(false);
            return wallet.Id;
        }

        /// <summary>
        /// Получение списка кошельков пользователя.
        /// </summary>
        /// <param name="ownerId">Идентификатор пользователя-владельца.</param>
        /// <param name="cancellationToken">Токен прерывания выполнения.</param>
        /// <returns>Массив кошельков пользователя.</returns>
        public async Task<WalletItemDto[]> GetUserWallets(Guid ownerId, CancellationToken cancellationToken)
        {
            var data = await context.Wallets.Find(r => r.OwnerId == ownerId).ToListAsync(cancellationToken).ConfigureAwait(false);
            return Mapper.Map<WalletItemDto[]>(data);
        }

        /// <summary>
        /// Удаление кошелька пользователя.
        /// </summary>
        /// <param name="walletId">Идентификатор кошелька.</param>
        /// <param name="cancellationToken">Токен прерывания выполнения.</param>
        /// <returns>Результат выполнения операции.</returns>
        /// <exception cref="ScException">Исключение, возвращаемое фронту через ПО промежуточного слоя.</exception>
        public async Task<bool> DeleteWallet(string walletId, CancellationToken cancellationToken)
        {
            var result = await context.Wallets.DeleteOneAsync(x => x.Id == walletId, cancellationToken).ConfigureAwait(false);
            if (!result.IsAcknowledged)
                throw new ScException($"Удаление кошелька \"{walletId}\" не подтверждено.");
            return true;
        }

        /// <summary>
        /// Изменение данных кошелька пользователя.
        /// </summary>
        /// <param name="walletId">Идентификатор кошелька пользователя.</param>
        /// <param name="name">Новое название кошелька.</param>
        /// <param name="isActive">Новый признак активности кошелька.</param>
        /// <param name="cancellationToken">Токен прерывания выполнения.</param>
        /// <returns>Результат выполнения операции.</returns>
        /// <exception cref="ScException">Исключение, возвращаемое фронту через ПО промежуточного слоя.</exception>
        public async Task<bool> UpdateWallet(string walletId, string name, bool isActive, CancellationToken cancellationToken)
        {
            var updateDefinition = new BsonDocument("$set", new BsonDocument
            {
                { "Name", name },
                { "IsActive", isActive }
            });
            var result = await context.Wallets.UpdateOneAsync(r => r.Id == walletId, updateDefinition, new UpdateOptions { IsUpsert = false }, cancellationToken).ConfigureAwait(false);
            if (!result.IsAcknowledged)
                throw new ScException($"Обновление данных кошелька \"{walletId}\" не подтверждено.");
            return true;
        }
    }
}
