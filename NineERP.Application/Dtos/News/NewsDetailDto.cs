namespace NineERP.Application.Dtos.News
{
    public class NewsDetailDto
    {
        public int Id { get; set; }

        public string Title { get; set; } = default!;
        public string? TitleVi { get; set; }

        public string? SubDescription { get; set; }
        public string? SubDescriptionVi { get; set; }

        public string? ImageUrl { get; set; }

        public string? Content { get; set; }
        public string? ContentVi { get; set; }

        public string? Tags { get; set; }

        public string? SeoPageTitle { set; get; }

        public string? SeoAlias { set; get; }

        public string? SeoAliasVi { set; get; }

        public string? SeoKeywords { set; get; }

        public string? SeoDescription { set; get; }

        public short Status { set; get; }
        public DateTime CreatedOn { get; set; }
        public List<NewsResponse> News { get; set; } = new ();
    }
}