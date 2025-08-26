using NineERP.Domain.Entities.Base;
using System.ComponentModel.DataAnnotations;

namespace NineERP.Domain.Entities.Kbn
{
    public class KbnDeviceType : AuditableBaseEntity<short>
    {
        [MaxLength(255)]
        public string NameEn { get; set; } = default!;
        [MaxLength(255)]
        public string NameVi { get; set; } = default!;
        [MaxLength(255)]
        public string NameJp { get; set; } = default!;
        public string? Description { get; set; }

    }
}
