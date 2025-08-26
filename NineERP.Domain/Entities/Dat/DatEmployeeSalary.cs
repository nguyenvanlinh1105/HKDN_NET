using NineERP.Domain.Entities.Base;
using System.ComponentModel.DataAnnotations;

namespace NineERP.Domain.Entities.Dat
{
    public class DatEmployeeSalary : AuditableBaseEntity<long>
    {
        [MaxLength(50)]
        public string EmployeeNo { get; set; } = default!;
        public double? SalaryBasic { get; set; }
        public double? SalaryGross { get; set; }
        public string? BankName { get; set; }
        public string? BankNumber { get; set; }
        public string? BankAccountName { get; set; }
        public short PaymentType { get; set; } // 0: Cash, 1: Bank Transfer
    }
}
