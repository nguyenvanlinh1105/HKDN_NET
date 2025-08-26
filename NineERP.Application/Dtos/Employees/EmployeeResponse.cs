namespace NineERP.Application.Dtos.Employees;
public class EmployeeResponse
{
    public long Id { get; set; }
    public string EmployeeNo { get; set; } = default!;
    public string FullName { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string? PhoneNo { get; set; }
    public string? AvatarUrl { get; set; }
    public string? PositionName { get; set; }
    public string? DepartmentName { get; set; }
    public string? Status { get; set; }
    public string? ContractTypeName { get; set; }
    public DateTime CreatedOn { get; set; }
}

