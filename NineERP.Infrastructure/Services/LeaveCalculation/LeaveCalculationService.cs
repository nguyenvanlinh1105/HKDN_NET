using Microsoft.EntityFrameworkCore;
using NineERP.Application.Extensions;
using NineERP.Application.Interfaces.Common;
using NineERP.Application.Interfaces.LeaveCalculation;
using NineERP.Application.Interfaces.Persistence;
using NineERP.Domain.Entities.Mst;

namespace NineERP.Infrastructure.Services.LeaveCalculation
{
    public class LeaveCalculationService(IApplicationDbContext context) : ILeaveCalculationService
    {
        /// <summary>
        /// Calculates total leave days considering working hours, holidays, and weekends
        /// </summary>
        public async Task<(float TotalDay, double TotalHour, string TotalHourText, List<(DateTime Date, bool IsHolidayAndSaturdayOrSunday, float LeaveHours)> DateLeaves)>
            CalculateTotalDayLeave(DateTime fromTime, DateTime toTime, string employeeNo, CancellationToken cancellationToken)
        {
            var dateLeaves = new List<(DateTime Date, bool IsHolidayAndSaturdayOrSunday, float LeaveHours)>();
            var fromDate = fromTime.Date;
            var toDate = toTime.Date;

            // Generate the range of dates for the leave period
            var dateRange = Enumerable.Range(0, (toDate - fromDate).Days + 1)
                .Select(offset => fromDate.AddDays(offset))
                .ToList();

            // Get all holidays in this date range using HashSet for optimized lookups
            var holidayDates = new HashSet<DateTime>(await context.DatCalendarHolidays
                .AsNoTracking()
                .Where(x => dateRange.Contains(new DateTime(x.Year, x.Month, x.Day)))
                .Select(x => new DateTime(x.Year, x.Month, x.Day))
                .ToListAsync(cancellationToken));

            // Get employee's default work shift
            var defaultShift = await (from ds in context.DatEmployeeShifts.AsNoTracking()
                                      join mstShift in context.MstShifts.AsNoTracking()
                                         on ds.ShiftId equals mstShift.Id into mstShiftGroup
                                      from mstShift in mstShiftGroup.DefaultIfEmpty()
                                      where ds.EmployeeNo == employeeNo && !ds.IsDeleted
                                      select mstShift).FirstOrDefaultAsync(cancellationToken);

            if (defaultShift == null)
                throw new ApiException("Shift config not found");

            var totalDay = 0f;
            var totalTime = TimeSpan.Zero;

            // Process each day in the date range
            foreach (var date in dateRange)
            {
                // Skip weekends
                if (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday)
                {
                    dateLeaves.Add((date, true, 0f));
                    continue;
                }

                // Skip holidays
                if (holidayDates.Contains(date))
                {
                    dateLeaves.Add((date, true, 0f));
                    continue;
                }

                // Calculate effective start and end times for this day
                DateTime effectiveStart = date == fromDate ? fromTime : date;
                DateTime effectiveEnd = date == toDate ? toTime : date.AddHours(23).AddMinutes(59);

                // Calculate portion of day and working hours
                var (portion, time) = CalculatePortionAndTime(effectiveStart, effectiveEnd, defaultShift);
                dateLeaves.Add((date, false, portion));
                totalDay += portion;
                totalTime += time;
            }

            return (
                TotalDay: totalDay,
                TotalHour: Math.Round(totalTime.TotalHours, 2),
                TotalHourText: $"{(int)totalTime.TotalHours}h{totalTime.Minutes}m",
                DateLeaves: dateLeaves
            );
        }

        /// <summary>
        /// Calculates the portion of a workday and actual working hours based on shift times
        /// </summary>
        private (float Portion, TimeSpan Duration) CalculatePortionAndTime(DateTime start, DateTime end, MstShift shift)
        {
            // Parse shift times
            var morningStart = TimeSpan.Parse(shift.MorningStartTime);
            var morningEnd = TimeSpan.Parse(shift.MorningEndTime);
            var afternoonStart = TimeSpan.Parse(shift.AfternoonStartTime);
            var afternoonEnd = TimeSpan.Parse(shift.AfternoonEndTime);

            var startTime = start.TimeOfDay;
            var endTime = end.TimeOfDay;

            var total = TimeSpan.Zero;
            float portion = 0f;

            // Check if leave includes morning shift
            if (endTime > morningStart && startTime < morningEnd)
            {
                var effectiveStart = Max(startTime, morningStart);
                var effectiveEnd = Min(endTime, morningEnd);
                var duration = effectiveEnd - effectiveStart;
                total += duration;
                if (duration.TotalMinutes > 0)
                    portion += 0.5f; // Morning counts as half day
            }

            // Check if leave includes afternoon shift
            if (endTime > afternoonStart && startTime < afternoonEnd)
            {
                var effectiveStart = Max(startTime, afternoonStart);
                var effectiveEnd = Min(endTime, afternoonEnd);
                var duration = effectiveEnd - effectiveStart;
                total += duration;
                if (duration.TotalMinutes > 0)
                    portion += 0.5f; // Afternoon counts as half day
            }

            return (portion, total);
        }

        /// <summary>
        /// Returns the maximum of two TimeSpan values
        /// </summary>
        private TimeSpan Max(TimeSpan a, TimeSpan b) => a > b ? a : b;

        /// <summary>
        /// Returns the minimum of two TimeSpan values
        /// </summary>
        private TimeSpan Min(TimeSpan a, TimeSpan b) => a < b ? a : b;
    }
}
