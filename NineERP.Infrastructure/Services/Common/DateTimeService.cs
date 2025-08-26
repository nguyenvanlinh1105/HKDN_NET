using System.Collections.ObjectModel;
using NineERP.Application.Interfaces.Common;

namespace NineERP.Infrastructure.Services.Common
{
    public class DateTimeService : IDateTimeService
    {
        public DateTime Now => DateTime.Now;
        TimeZoneInfo IDateTimeService.TimeZoneInfo => GetTimeZoneInfo();

        public TimeZoneInfo GetTimeZoneInfo()
        {
            ReadOnlyCollection<TimeZoneInfo> timeZones = TimeZoneInfo.GetSystemTimeZones();

            foreach (TimeZoneInfo timeZone in timeZones)
            {
                if (timeZone.Id == "Asia/Ho_Chi_Minh")
                {
                    return TimeZoneInfo.FindSystemTimeZoneById("Asia/Ho_Chi_Minh");
                }
            }
            return TimeZoneInfo.Local;
        }
    }
}