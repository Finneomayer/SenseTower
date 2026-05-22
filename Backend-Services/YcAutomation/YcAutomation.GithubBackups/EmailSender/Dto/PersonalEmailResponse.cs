using System;

namespace YcAutomation.GithubBackups.Services.EmailSender.Dto
{
    public class PersonalEmailResponse
    {
        public int Index { get; set; }

        public string Email { get; set; }

        public string Id { get; set; }

        public MailingError[] Errors { get; set; } = Array.Empty<MailingError>();
    }

    public class MailingError
    {
        public string Code { get; set; }

        public string Message { get; set; }
    }
}
