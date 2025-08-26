namespace NineERP.Application.Wrapper
{
    public class PaginatedResult<T> : Result
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public PaginatedResult(List<T> data)
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        {
            Data = data;
        }

        public List<T> Data { get; set; }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
#pragma warning disable IDE0060
        internal PaginatedResult(bool succeeded, List<T> data, List<string> messages, int count = 0, int page = 1, int pageSize = 10)
#pragma warning restore IDE0060
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        {
            Data = data;
            CurrentPage = page;
            Succeeded = succeeded;
            PageSize = pageSize;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
            TotalCount = count;
        }

        public static PaginatedResult<T> Failure(List<string> messages)
        {
#pragma warning disable IDE0028 // Simplify collection initialization
            return new PaginatedResult<T>(false, new List<T>(), messages);
#pragma warning restore IDE0028 // Simplify collection initialization
        }

        public static PaginatedResult<T> Success(List<T> data, int count, int page, int pageSize)
        {
#pragma warning disable IDE0028 // Simplify collection initialization
            return new PaginatedResult<T>(true, data, new List<string>(), count, page, pageSize);
#pragma warning restore IDE0028 // Simplify collection initialization
        }

        public int CurrentPage { get; set; }

        public int TotalPages { get; set; }

        public int TotalCount { get; set; }
        public int PageSize { get; set; }

        public bool HasPreviousPage => CurrentPage > 1;

        public bool HasNextPage => CurrentPage < TotalPages;
#pragma warning disable CS0108, CS0114
        public string LanguageType { get; set; }
#pragma warning restore CS0108, CS0114
    }
}