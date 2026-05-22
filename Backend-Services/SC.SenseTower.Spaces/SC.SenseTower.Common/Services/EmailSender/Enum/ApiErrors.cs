namespace SC.SenseTower.Common.Services.EmailSender.Enum
{
    public enum ApiErrors
    {
        unspecified,

        invalid_api_key,

        access_denied,

        unknown_method,

        invalid_arg,

        not_enough_money,

        retry_later,

        api_call_limit_exceeded_for_api_key,

        api_call_limit_exceeded_for_ip
    }
}
