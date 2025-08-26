using NineERP.Domain.Entities.Base;
using System.ComponentModel.DataAnnotations;

namespace NineERP.Domain.Entities.Kbn
{
    public class KbnContractType : AuditableBaseEntity<short>
    {
        public string? Description { get; set; }

        [MaxLength(255)]
        public string NameEn { get; set; } = default!;
        [MaxLength(255)]
        public string NameVi { get; set; } = default!;
        [MaxLength(255)]
        public string NameJp { get; set; } = default!;

        [MaxLength(50)]
        public string GroupCode { get; set; } = default!; // VD: "OFFICIAL", "INTERN", "FLEXIBLE"
    }
}
