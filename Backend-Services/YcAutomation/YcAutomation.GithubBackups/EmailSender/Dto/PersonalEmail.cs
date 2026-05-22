using YcAutomation.GithubBackups.Services.EmailSender.Interfaces;

namespace YcAutomation.GithubBackups.Services.EmailSender.Dto
{
    public class PersonalEmail : IBaseRequest
    {
        public string format { get; set; } = "json";

        public string api_key { get; set; }

        public int error_checking { get; set; } = 1;

        public string email { get; set; }

        public string sender_name { get; set; }

        public string sender_email { get; set; }

        public string subject { get; set; }

        public string body { get; set; }

        public long list_id { get; set; }

        public string lang { get; set; } = "ru";
    }
}
