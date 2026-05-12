using System;

namespace Assets.Scripts.Models
{
    public class CheckingTokenDto
    {
        public Guid UserId { get; set; }

        public string Token { get; set; } = null!;

        public string? Lang { get; set; }

        public DateTimeOffset? ServerRegistrationTime { get; set; }
    }
}