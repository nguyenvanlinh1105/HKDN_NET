namespace NineERP.Application.Dtos.KbnEmployeeStatus;

public class KbnEmployeeStatusRequest
{
    public string? Keyword { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string OrderBy { get; set; } = "CreatedOn desc";

}
