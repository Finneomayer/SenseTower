namespace SC.SenseTower.Accounts.Dto.Wallets
{
    public class WalletItemDto
    {
        /// <summary>
        /// Идентификатор кошелька.
        /// </summary>
        public string Id { get; set; } = null!;

        /// <summary>
        /// Название кошелька.
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Признак подтверждённого кошелька.
        /// </summary>
        public bool IsConfirmed { get; set; }

        /// <summary>
        /// Признак активного кошелька.
        /// </summary>
        public bool IsActive { get; set; }
    }
}
