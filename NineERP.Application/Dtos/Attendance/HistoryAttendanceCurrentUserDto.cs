using NineERP.Application.Wrapper;

namespace NineERP.Application.Dtos.Attendance
{
    public class HistoryAttendancesCurrentUserDto : PaginatedResultApi
    {
        public List<HistoryAttendanceCurrentUserDto>? HistoryAttendanceCurrentUser { get; set; }
        public int ModificationAttendanceCount { get; set; }
    }

    public class HistoryAttendanceCurrentUserDto
    {
        public long Id { get; set; }
        public DateTime WorkDay { get; set; }
        public DateTime? CheckInOn { get; set; }
        public DateTime? CheckOutOn { get; set; }
        public bool IsLate { get; set; }
        public bool IsLeaveSoon { get; set; }
        public string? Note { get; set; }
        public List<LeaveApplicationDto>? LeaveTypes { get; set; } = default!;
        public List<AnnualCalendarHolidayDto>? AnnualCalendarHolidays { get; set; } = default!;

    }

    public class LeaveApplicationDto
    {
        public long Id { get; set; }
        public string LeaveTypeEn { get; set; } = default!;
        public string LeaveTypeJa { get; set; } = default!;
        public string LeaveTypeVi { get; set; } = default!;
        public short Status { get; set; }
    }

    public class AnnualCalendarHolidayDto
    {
        public string Title { get; set; } = default!;
        public string TypeEn { get; set; } = default!;
        public string TypeVi { get; set; } = default!;
        public string TypeJa { get; set; } = default!;
    }
}
