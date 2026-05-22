namespace SC.SenseTower.Accounts.Settings
{
    public class IdentityServerSettings
    {
        public string BaseUrl { get; set; } = null!;

        public string ClientId { get; set; } = "vr";

        public int MaxAttempts { get; set; } = 5;

        public int BreakAfter { get; set; } = 3;

        public int BreakForSeconds { get; set; } = 30;

        public string IsLoginFreeUrl { get; set; } = null!;

        public string IsEmailFreeUrl { get; set; } = null!;

        public string RegisterUrl { get; set; } = null!;

        public string LookupUsersUrl { get; set; } = null!;

        public string DeleteUserUrl { get; set; } = null!;

        public string ConfirmEmailUrl { get; set; } = null!;

        public string CheckLoginOrEmailUrl { get; set; } = null!;

        public string SendResetPasswordUrl { get; set; } = null!;

        public string SetPasswordUrl { get; set; } = null!;

        public string IsEmailConfirmedUrl { get; set; } = null!;

        public string IsUserBlocked { get; set; } = null!;

        public string CheckToken { get; set; } = null!;

        public string UserInfoUrl { get; set; } = null!;

        public string UsersByIdsUrl { get; set; } = null!;
    }
}
