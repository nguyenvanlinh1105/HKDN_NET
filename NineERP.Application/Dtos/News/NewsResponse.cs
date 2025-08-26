namespace NineERP.Application.Dtos.News
{
    public class NewsResponse
    {
        public int Id { get; set; }

        public string Title { get; set; } = default!;

        public string TitleVi { get; set; } = default!;

        public string? SeoAlias { set; get; }

        public string? SeoAliasVi { set; get; }

        public short Status { set; get; }

        public string CreatedBy { get; set; } = default!;

        public DateTime CreatedOn { get; set; }

        public string? ImageUrl { get; set; }
    }
}