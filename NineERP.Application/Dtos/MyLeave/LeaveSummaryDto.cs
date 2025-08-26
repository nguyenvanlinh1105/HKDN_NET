namespace NineERP.Application.Dtos.MyLeave
{
    public class LeaveSummaryDto
    {
        public decimal TotalLeaveCanUse { get; set; }
        public decimal TotalLeaveUsed { get; set; }
        public decimal TotalSpecialLeaveDays { get; set; }
        public decimal TotalRemainingLeave { get; set; }
        public LeaveMasterData LeaveMasterData { get; set; } = new();
    }

    public class LeaveMasterData
    {
        public List<LeaveTypeDto> LeaveTypes { get; set; } = new();
        public EmployeeShiftDto? EmployeeShift { get; set; } = new();
        public List<HolidayDto>? Holidays { get; set; } = new();
        public List<EmployeeApproveDto>? EmployeeApproves { get; set; } = new();
    }
    public class LeaveTypeDto
    {
        public string NameEn { get; set; } = default!;
        public string NameVi { get; set; } = default!;
        public string NameJa { get; set; } = default!;
        public string? Color { get; set; }
        public short TypeFlag { get; set; }
        public string Acronym { get; set; } = default!;
    }

    public class EmployeeShiftDto
    {
        public string MorningStartTime { get; set; } = default!;
        public string MorningEndTime { get; set; } = default!;
        public string AfternoonStartTime { get; set; } = default!;
        public string AfternoonEndTime { get; set; } = default!;
        public string TotalHour { get; set; } = default!;
    }

    public class HolidayDto
    {
        public string Title { get; set; } = default!;
        public DateTime Date { get; set; } = default!;
        public int TimeOfDay { get; set; }
    }

    public class EmployeeApproveDto
    {
        public short ApproveLevel { get; set; } = default!;
        public short ApproveType { get; set; } = default!;
        public string EmployeeNoApprove { get; set; } = default!;
        public string EmployeeApproveName { get; set; } = default!;
    }
}
