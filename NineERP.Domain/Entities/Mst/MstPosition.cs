using NineERP.Domain.Entities.Base;
using System.ComponentModel.DataAnnotations;

namespace NineERP.Domain.Entities.Mst
{
    public class MstPosition : AuditableBaseEntity<short>
    {
        [MaxLength(255)]
        public string NameEn { get; set; } = default!;
        [MaxLength(255)]
        public string NameVi { get; set; } = default!;
        [MaxLength(255)]
        public string NameJa { get; set; } = default!;
        [MaxLength(255)]
        public string? Description { get; set; }
    }
}
