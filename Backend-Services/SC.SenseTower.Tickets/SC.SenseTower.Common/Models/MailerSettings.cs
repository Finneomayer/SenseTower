namespace SC.SenseTower.Common.Models
{
    public class MailerSettings
    {
        public string RootUrl { get; set; } = null!;

        public int MaxAttempts { get; set; }

        public int BreakAfter { get; set; }

        public int BreakForSeconds { get; set; }

        public string AddressFrom { get; set; } = null!;

        public string NameFrom { get; set; } = null!;

        public string ApiKey { get; set; } = null!;

        public long ContactListId { get; set; }

        public string SendEmailUrl { get; set; } = null!;
    }
}
