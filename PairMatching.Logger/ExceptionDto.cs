namespace PairMatching.Loggin
{
    public record ExceptionDto
    {
        public ExceptionDto(Exception exception)
        {
            if(exception != null)
            {
                Message = exception.Message;
                Source = exception.Source;
                StackTrace = exception.StackTrace;
            }
        }

        public ExceptionDto() { }

        public string Message { get; } = "";

        public string? Source { get; } = "";

        public string StackTrace { get; set; } = "";
    }
}