using NineERP.Application.Wrapper;

namespace NineERP.Application.Dtos.Task
{
    public class CommentsTaskDto : PaginatedResultApi
    {
        public List<CommentTaskDto> Comments { get; set; } = default!;
    }

    public class CommentTaskDto
    {
        public int Id { get; set; }
        public string Comment { get; set; } = default!;
        public string EmployeeNo { get; set; } = default!;
        public string NickName { get; set; } = default!;
        public string? ImageUrl { get; set; }
        public DateTime CreatedDate { get; set; } = default!;
    }

    public class AddCommentTaskDto
    {
        public int TaskId { get; set; }
        public string Comment { get; set; } = default!;
    }
}
