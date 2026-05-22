namespace SC.SenseTower.Auth.Settings
{
    public class MailerSettings
    {
        public string RootUrl { get; set; }

        public int MaxAttempts { get; set; }

        public int BreakAfter { get; set; }

        public int BreakForSeconds { get; set; }

        public string AddressFrom { get; set; }

        public string NameFrom { get; set; }

        public string ApiKey { get; set; }

        public long ContactListId { get; set; }

        public string SendEmailUrl { get; set; }
    }
}
