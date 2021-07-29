namespace API.Errors
{
    public class ApiException
    {
        private string v;

        public ApiException(int statusCode, string v)
        {
            StatusCode = statusCode;
            this.v = v;
        }

        public ApiException(int statusCode, string message, string details)
        {
            this.StatusCode = statusCode;
            this.Message = message;
            this.Details = details;

        }

        public int StatusCode { get; set; }
        public string Message { get; set; }
        public string Details { get; set; }
    }
    
}