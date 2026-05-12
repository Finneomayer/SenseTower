namespace Assets.Scripts.Models
{
    public sealed class HttpResponse<T>
    {
        public const long SuccessCode = 200;
        public const long NotAuthorizedCode = 401;

        public T ResponseData { get; }
        public long ResponseCode { get; }

        public HttpResponse(T responseData, long responseCode)
        {
            ResponseData = responseData;
            ResponseCode = responseCode;
        }
    }

    public sealed class EmptyResponseData
    {
    }
}