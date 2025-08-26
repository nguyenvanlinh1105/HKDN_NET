using NineERP.Domain.Entities.Base;
using System.ComponentModel.DataAnnotations;

namespace NineERP.Domain.Entities.Dat
{
    public class DatProjectDetail : AuditableBaseEntity<int>
    {
        public int ProjectId { get; set; }
        public short ProjectDetailType { get; set; }
        [MaxLength(50)]
        public string EmployeeNo { get; set; } = default!;
        public DateTime JoinDate { get; set; }
        public DateTime? LeaveDate { get; set; }
        public short RoleId { get; set; }
        public short Status { get; set; }
        public string? Note { get; set; }
    }
}
