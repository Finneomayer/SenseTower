using SC.SenseTower.Common.Services.EmailSender.Enum;

namespace SC.SenseTower.Common.Services.EmailSender.Dto
{
    public class PersonalEmailResponse
    {
        public int Index { get; set; }

        public string? Email { get; set; }

        public string? Id { get; set; }

        public MailingError[] Errors { get; set; } = Array.Empty<MailingError>();
    }

    public class MailingError
    {
        public MailingErrors Code { get; set; }

        public string? Message { get; set; }
    }
}
