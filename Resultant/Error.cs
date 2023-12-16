namespace Resultant
{
    public class Error
    {
        public string Message { get; }
        public int Code { get; }

        public Error(string message, int code = 0)
        {
            Message = message;
            Code = code;
        }
    }
}