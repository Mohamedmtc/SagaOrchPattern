namespace OrderService
{
    public class WebApiException : Exception
    {
        public List<string>? Errors { get; set; } = null;
        public WebApiException(string Message) : base(message: Message) { }
        public WebApiException(string Message, List<string> errors) : base(message: Message)
        {
            Errors = errors;
        }
    }
}
