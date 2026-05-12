namespace Models
{
    public class ScResult
    {
        public ScError? Error { get; set; }

        public ScResult()
        {
        }

        public ScResult(ScError? error)
        {
            Error = error;
        }
    }

    public class ScResult<T> : ScResult
    {
        public ScResult()
        {
        }

        public ScResult(ScError error) : base(error)
        {
        }

        public ScResult(T result) : base(null)
        {
            Result = result;
        }

        public T? Result { get; set; }
    }
}