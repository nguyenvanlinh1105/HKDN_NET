namespace NineERP.Application.Dtos.MstDepartment;

public class DepartmentDetailDto
{
    public int Id { get; set; }

    public string NameVi { get; set; } = default!;
    public string NameEn { get; set; } = default!;
    public string NameJa { get; set; } = default!;

    public int? ParentId { get; set; }
    public string? Description { get; set; }
}
