using AutoMapper;
using MongoDB.Driver;
using SC.SenseTower.Accounts.Data;
using SC.SenseTower.Accounts.Data.Models;
using SC.SenseTower.Common.Exceptions;
using SC.SenseTower.Common.Services;

namespace SC.SenseTower.Accounts.Services
{
    public class TicketsService : BaseDbService
    {
        private new AccountsDbContext context => base.context as AccountsDbContext ?? null!;

        public TicketsService(
            ILogger<TicketsService> logger,
            IMapper mapper,
            AccountsDbContext context) : base(logger, mapper, context)
        {
        }

        /// <summary>
        /// Отзыв билета.
        /// </summary>
        /// <param name="ticketId">Идентификатор (код) билета.</param>
        /// <param name="reason">Причина отзыва.</param>
        /// <param name="cancellationToken">Токен прерывания выполнения.</param>
        /// <returns></returns>
        /// <exception cref="ScException"></exception>
        public async Task<bool> Recall(string? ticketId, string? reason, CancellationToken cancellationToken)
        {
            var filter = Builders<Ticket>.Filter.Eq(r => r.Id, ticketId);
            var update = Builders<Ticket>.Update
                .Set(r => r.RecallInfo.IsRecalled, true)
                .Set(r => r.RecallInfo.Date, DateTime.UtcNow)
                .Set(r => r.RecallInfo.RecallReason, reason);
            var updateResult = await context.Tickets.UpdateOneAsync(filter, update, new UpdateOptions { BypassDocumentValidation = true }, cancellationToken);
            if (updateResult.ModifiedCount == 0)
                throw new ScException("Ошибка отзыва билета.");
            return true;
        }

        /// <summary>
        /// Подсчет количества билетов пользователя.
        /// </summary>
        /// <param name="userId">Идентификатор пользователя.</param>
        /// <param name="cancellationToken">Токен прерывания выполнения.</param>
        /// <returns>Число билетов, выданных пользователю.</returns>
        public async Task<int> CountByUser(Guid userId, CancellationToken cancellationToken)
        {
            var filter = Builders<Ticket>.Filter.Eq(x => x.IssuerId, userId);
            return (int)await context.Tickets.Find(filter).CountDocumentsAsync(cancellationToken);
        }

        /// <summary>
        /// Создание пакета билетов для пользователя.
        /// </summary>
        /// <param name="userId">Идентификатор пользователя-владельца билета.</param>
        /// <param name="quantity">Количество билетов в пакете.</param>
        /// <param name="cancellationToken">Токен прерывания выполнения.</param>
        /// <returns></returns>
        public async Task<string[]> CreateUserTickets(Guid userId, int quantity, CancellationToken cancellationToken)
        {
            var tickets = Enumerable.Range(1, quantity)
                .Select(_ => new Ticket
                {
                    Id = Guid.NewGuid().ToString().Replace("-", ""),
                    IssuerId = userId
                })
                .ToArray();
            await context.Tickets.InsertManyAsync(tickets, new InsertManyOptions { BypassDocumentValidation = true }, cancellationToken);
            return tickets.Select(r => r.Id).ToArray();
        }

        /// <summary>
        /// Получение билета по его идентификатору.
        /// </summary>
        /// <param name="ticketId">Идентификатор билета.</param>
        /// <param name="cancellationToken">Токен прерывания выполнения.</param>
        /// <returns>Сущность билета</returns>
        public async Task<Ticket?> Get(string ticketId, CancellationToken cancellationToken)
        {
            var filter = Builders<Ticket>.Filter.Eq(r => r.Id, ticketId);
            return await context.Tickets.Find(filter).FirstOrDefaultAsync(cancellationToken);
        }

        /// <summary>
        /// Пометка билета как использованного указанным пользователем.
        /// </summary>
        /// <param name="ticketId">Идентификатор (код) билета.</param>
        /// <param name="userId">Идентификатор пользователя.</param>
        /// <param name="cancellationToken">Токен прерывания выполнения.</param>
        /// <returns></returns>
        /// <exception cref="ScException">Исключение, обрабатываемое ПО промежуточного слоя.</exception>
        public async Task MarkAsUsed(string ticketId, Guid userId, CancellationToken cancellationToken)
        {
            var filter = Builders<Ticket>.Filter.Eq(r => r.Id, ticketId);
            var update = Builders<Ticket>.Update
                .Set(r => r.UsingInfo.UserId, userId)
                .Set(r => r.UsingInfo.Date, DateTime.UtcNow);
            var updateResult = await context.Tickets.UpdateOneAsync(filter, update, new UpdateOptions { BypassDocumentValidation = true }, cancellationToken);
            if (updateResult.ModifiedCount == 0)
                throw new ScException("Ошибка установки признака использованного билета.");
        }

        /// <summary>
        /// Получение списка билетов пользователя.
        /// </summary>
        /// <param name="userId">Идентификатор пользователя.</param>
        /// <param name="cancellationToken">Токен прерывания выполнения.</param>
        /// <returns>Массив билетов пользователя.</returns>
        public async Task<Ticket[]> GetUserTickets(Guid userId, CancellationToken cancellationToken)
        {
            var filter = Builders<Ticket>.Filter.Eq(r => r.IssuerId, userId);
            var result = await context.Tickets.Find(filter).ToListAsync(cancellationToken);
            return result.ToArray();
        }
    }
}
