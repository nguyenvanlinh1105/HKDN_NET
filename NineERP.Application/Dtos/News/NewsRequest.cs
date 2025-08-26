using NineERP.Application.Request;

namespace NineERP.Application.Dtos.News
{
    public class NewsRequest : RequestParameter
    {
        public short? Status { get; set; }
    }
}