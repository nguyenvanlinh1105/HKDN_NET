using System.ComponentModel.DataAnnotations;
using NineERP.Domain.Entities.Base;

namespace NineERP.Domain.Entities.Mst
{
    public class MstDepartment : AuditableBaseEntity<short>
    {
        [MaxLength(255)]
        public string NameVi { get; set; } = default!;
        
        [MaxLength(255)]
        public string NameEn { get; set; } = default!;
        
        [MaxLength(255)]
        public string NameJa { get; set; } = default!;
        public short? ParentId { get; set; } 
        public string? Description { get; set; }
    }
}
