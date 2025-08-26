using NineERP.Domain.Entities.Base;
using System.ComponentModel.DataAnnotations;

namespace NineERP.Domain.Entities.Dat
{
    public class DatEmployeeLogTime : AuditableBaseEntity<long>
    {
        /// <summary>
        /// Employee No
        /// </summary>
        [MaxLength(50)]
        public string EmployeeNo { get; set; } = default!;

        /// <summary>
        /// Work Day
        /// </summary>
        public DateTime WorkDay { get; set; }

        /// <summary>
        /// Check In On
        /// </summary>
        public DateTime? CheckInOn { get; set; }

        /// <summary>
        /// Check Out On
        /// </summary>
        public DateTime? CheckOutOn { get; set; }

        /// <summary>
        /// Is Update
        /// </summary>
        public bool? IsUpdate { get; set; }

        /// <summary>
        /// Note
        /// </summary>
        [MaxLength(1000)]
        public string? Note { get; set; }

        /// <summary>
        /// Is Check Late
        /// </summary>
        public bool? IsCheckLate { get; set; }

        /// <summary>
        /// Is Check Leave Soon
        /// </summary>
        public bool? IsCheckLeaveSoon { get; set; }
    }
}
