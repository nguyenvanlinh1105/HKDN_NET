using NineERP.Domain.Entities.Base;
using System.ComponentModel.DataAnnotations;

namespace NineERP.Domain.Entities.Dat
{
    public class DatEmployeeShift : AuditableBaseEntity<long>
    {
        public int ShiftId { get; set; }
        [MaxLength(50)]
        public string EmployeeNo { get; set; } = default!;
    }
}
