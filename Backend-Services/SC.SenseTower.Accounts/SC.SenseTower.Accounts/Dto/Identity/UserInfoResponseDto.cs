using System.Text.Json.Serialization;

namespace SC.SenseTower.Accounts.Dto.Identity
{
    public class UserInfoResponseDto
    {
        [JsonPropertyName("sub")]
        public Guid UserId { get; set; }

        [JsonPropertyName("role")]
        public string Role { get; set; } = null!;

        [JsonPropertyName("preferred_username")]
        public string PreferredUserName { get; set; } = null!;

        [JsonPropertyName("name")]
        public string Login { get; set; } = null!;

        [JsonPropertyName("email")]
        public string Email { get; set; } = null!;
    }
}
