using NineERP.Application.Wrapper;

namespace NineERP.Application.Dtos.Task
{
    public class DocumentsTaskDto : PaginatedResultApi
    {
        public List<DocumentTaskDto> Documents { get; set; } = default!;
    }

    public class DocumentTaskDto
    {
        public int Id { get; set; }
        public string NameFile { get; set; } = default!;
        public string TypeFile { get; set; } = default!;
        public string SizeFile { get; set; } = default!;
        public string FileUrl { get; set; } = default!;
    }
}
