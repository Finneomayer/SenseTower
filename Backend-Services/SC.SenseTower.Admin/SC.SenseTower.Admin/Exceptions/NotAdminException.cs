namespace SC.SenseTower.Admin.Exceptions
{
    public class NotAdminException : Exception
    {
        public NotAdminException() : base() { }

        public NotAdminException(string message) : base(message) { }

        public NotAdminException(string message, Exception innerException) : base(message, innerException) { }
    }
}
