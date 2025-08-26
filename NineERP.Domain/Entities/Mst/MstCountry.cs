using NineERP.Domain.Entities.Base;
using System.ComponentModel.DataAnnotations;

namespace NineERP.Domain.Entities.Mst
{
    public class MstCountry : AuditableBaseEntity<short>
    {
        [MaxLength(255)]
        public string Name { get; set; } = default!;
    }
}
