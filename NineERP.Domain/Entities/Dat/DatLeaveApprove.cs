using NineERP.Domain.Entities.Base;
using System.ComponentModel.DataAnnotations;

namespace NineERP.Domain.Entities.Dat
{
    public class DatLeaveApprove : AuditableBaseEntity<long>
    {
        public long EmployeeLeaveId { get; set; }
        public long EmployeeApproveId { get; set; }
        [MaxLength(50)]
        public string EmployeeNoApprove { get; set; } = default!;
        public short StatusApprove { get; set; }
        public short ApproveLevel { get; set; }
        public short ApproveType { get; set; }
        public string Note { get; set; } = default!;
        public DateTime? DateApprove { get; set; }
    }
}
