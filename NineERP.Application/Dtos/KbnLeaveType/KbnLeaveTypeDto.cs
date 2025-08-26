namespace NineERP.Application.Dtos.KbnLeaveType;

public class KbnLeaveTypeDto
{
    public int Id { get; set; }
    public string NameVi { get; set; } = default!;
    public string NameEn { get; set; } = default!;
    public string NameJp { get; set; } = default!;
    public string? Description { get; set; }
    public string LeaveTypeFlag { get; set; } = default!;
    public string Acronym { get; set; } = default!;
}
