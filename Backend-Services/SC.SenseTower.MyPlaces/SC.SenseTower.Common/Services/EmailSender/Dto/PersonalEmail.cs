using SC.SenseTower.Common.Services.EmailSender.Interfaces;

namespace SC.SenseTower.Common.Services.EmailSender.Dto
{
    public class PersonalEmail : IBaseRequest
    {
        public string format { get; set; } = "json";

        public string api_key { get; set; } = null!;

        public int error_checking { get; set; } = 1;

        public string email { get; set; } = null!;

        public string? sender_name { get; set; }

        public string sender_email { get; set; } = null!;

        public string subject { get; set; } = null!;

        public string body { get; set; } = null!;

        public long list_id { get; set; }

        public string lang { get; set; } = "ru";
    }
}
