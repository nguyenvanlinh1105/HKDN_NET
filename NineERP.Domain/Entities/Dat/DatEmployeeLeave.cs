using NineERP.Domain.Entities.Base;
using System.ComponentModel.DataAnnotations;

namespace NineERP.Domain.Entities.Dat
{
    public class DatEmployeeLeave : AuditableBaseEntity<long>
    {
        [MaxLength(50)]
        public string EmployeeNo { get; set; } = default!;
        public string Reason { get; set; } = default!;
        public short LeaveTypeId { get; set; }
        public short Status { get; set; }
        public DateTime FromTime { get; set; }
        public DateTime ToTime { get; set; }
        public decimal TotalDay { get; set; }
        public string TotalHour { get; set; } = default!;
        public double? TotalTime { get; set; }
    }
}
