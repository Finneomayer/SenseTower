using System;

namespace Mechanics.UserWallet.Model
{
    public class TwrsTransferTransaction
    {
        /// <summary>
        /// Идентификатор транзакции
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// Кошелек из которого идет перевод
        /// </summary>
        public TwrWallet? From { get; set; }
        /// <summary>
        /// Кошелек в который идет перевод
        /// </summary>
        public TwrWallet? To { get; set; }
        /// <summary>
        /// Сумма перевода
        /// </summary>
        public decimal Amount { get; set; }
        /// <summary>
        /// Время транзакции
        /// </summary>
        public DateTimeOffset Timestamp { get; set; }
    }
}