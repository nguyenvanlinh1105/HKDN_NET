using NineERP.Domain.Entities.Base;

namespace NineERP.Domain.Entities.Mst
{
    public class MstShift : AuditableBaseEntity<short>
    {
        /// <summary>
        /// Morning Start Time
        /// </summary>
        public string MorningStartTime { get; set; } = default!;

        /// <summary>
        /// Morning End Time
        /// </summary>
        public string MorningEndTime { get; set; } = default!;

        /// <summary>
        /// Afternoon Start Time
        /// </summary>
        public string AfternoonStartTime { get; set; } = default!;

        /// <summary>
        /// Morning End Time
        /// </summary>
        public string AfternoonEndTime { get; set; } = default!;

        /// <summary>
        /// Total Hour
        /// </summary>
        public string TotalHour { get; set; } = default!;

        /// <summary>
        /// Description
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Is Default
        /// </summary>
        public bool IsDefault { get; set; }
    }
}
