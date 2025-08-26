namespace NineERP.Application.Dtos.MstDepartment;

public class DepartmentTreeDto
{
    public int Id { get; set; }

    public string NameVi { get; set; } = default!;
    public string NameEn { get; set; } = default!;
    public string NameJa { get; set; } = default!;

    public List<DepartmentTreeDto> Children { get; set; } = new();
}
