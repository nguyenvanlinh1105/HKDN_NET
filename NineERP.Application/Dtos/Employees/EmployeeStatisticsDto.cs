namespace NineERP.Application.Dtos.Employees;

public class EmployeeStatisticsDto
{
    public int Total { get; set; }
    public int Official { get; set; }
    public int Probation { get; set; }
    public int Intern { get; set; }
    public int Temporary { get; set; }
    public int Flexible { get; set; }
    public int Project { get; set; }
}

