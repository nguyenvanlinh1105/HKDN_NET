namespace NineERP.Application.Dtos.MstProgrammingLanguage;

public class MstProgrammingLanguageRequest
{
    public string? Keyword { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string OrderBy { get; set; } = "CreatedOn desc";

}
