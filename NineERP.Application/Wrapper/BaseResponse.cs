namespace NineERP.Application.Wrapper
{
    public abstract class BaseResponse(int statusCode, string status, string? message = null)
    {
        public int StatusCode { get; set; } = statusCode;
        public string Status { get; set; } = status;
        public string? Message { get; set; } = message;
    }
}
