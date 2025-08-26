namespace NineERP.Application.Dtos.KbnEmployeeStatus;

public class KbnEmployeeStatusDto
{
    public int Id { get; set; }
    public string Status { get; set; } = default!;
    public string NameVi { get; set; } = default!;
    public string NameEn { get; set; } = default!;
    public string NameJp { get; set; } = default!;
    public string? Description { get; set; }
}
