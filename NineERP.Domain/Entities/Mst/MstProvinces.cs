using NineERP.Domain.Entities.Base;
using System.ComponentModel.DataAnnotations;

namespace NineERP.Domain.Entities.Mst
{
    public class MstProvinces : AuditableBaseEntity<short>
    {
        public short CountryId { get; set; }
        [MaxLength(255)]
        public string Name { get; set; } = default!;
    }
}
