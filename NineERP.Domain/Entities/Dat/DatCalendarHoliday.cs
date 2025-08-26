using NineERP.Domain.Entities.Base;

namespace NineERP.Domain.Entities.Dat
{
    public class DatCalendarHoliday : AuditableBaseEntity<short>
    {
        /// <summary>
        /// Year
        /// </summary>
        public int Year { get; set; }

        /// <summary>
        /// Month
        /// </summary>
        public int Month { get; set; }

        /// <summary>
        /// Day
        /// </summary>
        public int Day { get; set; }

        /// <summary>
        /// Time Of Day
        /// </summary>
        public int TimeOfDay { get; set; }

        /// <summary>
        /// Description
        /// </summary>
        public string Description { get; set; } = default!;

        /// <summary>
        /// HolidayTypeId foreign key reference MstHolidayType
        /// </summary>
        public short HolidayTypeId { get; set; }
    }
}
