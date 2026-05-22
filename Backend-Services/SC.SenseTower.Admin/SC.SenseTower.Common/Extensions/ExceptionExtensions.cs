namespace SC.SenseTower.Common.Extensions
{
    public static class ExceptionExtensions
    {
        public static string GetMessage(this Exception ex)
        {
            return ex.InnerException == null ? ex.Message : $"{ex.Message}: {ex.InnerException.Message}";
        }
    }
}
