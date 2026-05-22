namespace SC.SenseTower.Accounts.Data.Models
{
    public class RecallInfo
    {
        public bool IsRecalled { get; set; }

        public DateTime? Date { get; set; }

        public string? RecallReason { get; set; }
    }
}
