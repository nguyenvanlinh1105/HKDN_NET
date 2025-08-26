using NineERP.Domain.Entities.Base;
using System.ComponentModel.DataAnnotations;

namespace NineERP.Domain.Entities.Dat
{
    public class DatEmployeeAnnualLeave : AuditableBaseEntity<long>
    {
        [MaxLength(50)]
        public string EmployeeNo { get; set; } = default!;
        public decimal LeaveCurrentYear { get; set; }
        public decimal LeaveUsed { get; set; }
        public decimal LeaveLastYear { get; set; }
    }
}
