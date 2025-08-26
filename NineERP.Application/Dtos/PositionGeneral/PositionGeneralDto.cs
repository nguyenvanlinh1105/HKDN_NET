using System.ComponentModel.DataAnnotations;

namespace NineERP.Application.Dtos.PositionGeneral
{
    public class PositionGeneralDto
    {
        public string? TeamNameEn { get; set; }
        public string? TeamNameJp { get; set; }
        public string? TeamNameVi { get; set; }
        public string? ContractTypeNameEn { get; set; }
        public string? ContractTypeNameJp { get; set; }
        public string? ContractTypeNameVi { get; set; }
        public string? EmployeeStatusEn { get; set; }
        public string? EmployeeStatusJp { get; set; }
        public string? EmployeeStatusVi { get; set; }
        public DateTime? WorkingDateFrom { get; set; }
        public DateTime? WorkingDateTo { get; set; }
        public DateTime? ContractFrom { get; set; }
    }

    public class PositionDto
    {
        public short Id { get; set; }
        public string NameEn { get; set; } = default!;
        public string NameVi { get; set; } = default!;
        public string NameJa { get; set; } = default!;
    }
}
