namespace NineERP.Application.Interfaces.LeaveCalculation
{
    public interface ILeaveCalculationService
    {
        /// <summary>
        /// Calculates total leave days, hours, and details for a given period
        /// </summary>
        Task<(float TotalDay, double TotalHour, string TotalHourText, List<(DateTime Date, bool IsHolidayAndSaturdayOrSunday, float LeaveHours)> DateLeaves)>
            CalculateTotalDayLeave(DateTime fromTime, DateTime toTime, string employeeNo, CancellationToken cancellationToken);
    }
}
