using NineERP.Domain.Entities.Base;
using System.ComponentModel.DataAnnotations;

namespace NineERP.Domain.Entities.Dat
{
    public class DatEmployeeLeaveDetail : AuditableBaseEntity<long>
    {
        public long EmployeeLeaveId { get; set; }
        [MaxLength(50)]
        public string EmployeeNo { get; set; } = default!;
        public int Year { get; set; }
        public int Month { get; set; }
        public double TotalDay { get; set; }
    }
}
