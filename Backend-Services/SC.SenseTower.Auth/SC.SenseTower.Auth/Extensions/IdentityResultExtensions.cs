using Microsoft.AspNetCore.Identity;

namespace SC.SenseTower.Auth.Extensions
{
    public static class IdentityResultExtensions
    {
        public static string GetMessages(this IdentityResult identityResult)
        {
            return string.Join("; ", identityResult.Errors.Select(r => $"{r.Code}: {r.Description}"));
        }
    }
}
