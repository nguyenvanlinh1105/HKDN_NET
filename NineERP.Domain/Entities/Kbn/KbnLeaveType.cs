using System.ComponentModel.DataAnnotations;
using NineERP.Domain.Entities.Base;

namespace NineERP.Domain.Entities.Kbn
{
    public class KbnLeaveType : AuditableBaseEntity<short>
    {
        [MaxLength(255)]
        public string NameEn { get; set; } = default!;
        [MaxLength(255)]
        public string NameVi { get; set; } = default!;
        [MaxLength(255)]
        public string NameJp { get; set; } = default!;
        /// <summary>
        /// Leave Type Color
        /// </summary>
        public string? Color { get; set; }
        /// <summary>
        /// Leave Type Description
        /// </summary>
        public string? Description { get; set; }
        /// <summary>
        /// Leave Type Flag
        /// </summary>
        public short LeaveTypeFlag { get; set; }
        /// <summary>
        /// Acronym
        /// </summary>
        public string Acronym { get; set; } = default!;
    }
}
