namespace SC.SenseTower.Common.HttpClientLogging
{
    public static class HeaderLoggingExtensions
    {
        public static string RemoveHeader (this string request, string header)
        {
            if (!String.IsNullOrEmpty(request) && !String.IsNullOrEmpty(header))
            {
                var headerIndex = request.IndexOf($"{header}: ", StringComparison.OrdinalIgnoreCase);
                if (headerIndex > 0)
                {
                    var headerLastIndex = request.IndexOf(Environment.NewLine, headerIndex + header.Length, StringComparison.OrdinalIgnoreCase);
                    if (headerIndex < headerLastIndex)
                        request = request.Remove(headerIndex, headerLastIndex - headerIndex + Environment.NewLine.Length);
                }
            }

            return request;
        }
            
    }
}