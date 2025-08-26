using NineERP.Domain.Entities.Base;
using System.ComponentModel.DataAnnotations;

namespace NineERP.Domain.Entities.Dat
{
    public class DatEmployeeApprove : AuditableBaseEntity<long>
    {
        [MaxLength(50)]
        public string EmployeeNoLeave { get; set; } = default!;
        [MaxLength(50)]
        public string EmployeeNoApprove { get; set; } = default!;
        public short ApproveLevel { get; set; }
        public short ApproveType { get; set; }
    }
}
