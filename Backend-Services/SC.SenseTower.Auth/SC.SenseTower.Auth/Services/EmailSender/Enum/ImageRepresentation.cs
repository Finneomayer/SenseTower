using System.Text.Json.Serialization;

namespace SC.SenseTower.Auth.Services.EmailSender.Enum
{
    public enum ImageRepresentation
    {
        [JsonPropertyName("attachments")]
        Attachments,

        [JsonPropertyName("only_links")]
        OnlyLinks,

        [JsonPropertyName("user_default")]
        UserDefault
    }
}
