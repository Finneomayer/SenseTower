namespace SC.SenseTower.Accounts.Dto.Invites
{
    public class RecallInfoDto
    {
        /// <summary>
        /// Признак отозванного приглашения.
        /// </summary>
        public bool IsRecalled { get; set; }

        /// <summary>
        /// Универсальные дата и время отзыва.
        /// </summary>
        public DateTime? Date { get; set; }

        /// <summary>
        /// Причина отзыва.
        /// </summary>
        public string? RecallReason { get; set; }
    }
}
