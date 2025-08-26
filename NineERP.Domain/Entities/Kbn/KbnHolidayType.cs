using System.ComponentModel.DataAnnotations;
using NineERP.Domain.Entities.Base;

namespace NineERP.Domain.Entities.Kbn
{
    public class KbnHolidayType : AuditableBaseEntity<short>
    {
        [MaxLength(255)]
        public string NameEn { get; set; } = default!;
        [MaxLength(255)]
        public string NameVi { get; set; } = default!;
        [MaxLength(255)]
        public string NameJp { get; set; } = default!;
        /// <summary>
        /// Leave Type Description
        /// </summary>
        public string? Description { get; set; }

    }
}
