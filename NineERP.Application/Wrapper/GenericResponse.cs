namespace NineERP.Application.Wrapper
{
    public class GenericResponse<T>
    {
        public int Status { get; set; }
        public bool Success { get; set; }
        public string? Message { get; set; }
        public T? Data { get; set; }
        public List<ErrorDetail>? Errors { get; set; }

        private GenericResponse(int status, bool success, string message, T? data, List<ErrorDetail>? errors = null)
        {
            Status = status;
            Success = success;
            Message = message;
            Data = data;
            Errors = errors;
        }

        public static GenericResponse<T> SuccessResponse(int status, string message, T data) =>
            new(status, true, message, data);

        public static GenericResponse<T> ErrorResponse(int status, string message, string code, string details) =>
            new(status, false, message, default, [new ErrorDetail { Code = code, Details = details }]);

        public static GenericResponse<T> MultipleErrorsResponse(int status, string message, List<ErrorDetail> errors) =>
            new(status, false, message, default, errors);
    }

    public class ErrorDetail
    {
        public string Code { get; set; } = string.Empty;
        public string Details { get; set; } = string.Empty;
    }
}