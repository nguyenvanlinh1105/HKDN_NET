using NineERP.Application.Wrapper;

namespace NineERP.Application.Dtos.MyLeave
{
    public class MyLeavesDto : PaginatedResultApi
    {
        public List<MyLeaveDto>? MyLeaves { get; set; }
    }
    public class MyLeaveDto
    {
        public long Id { get; set; }
        public string Reason { get; set; } = default!;
        public short LeaveTypeId { get; set; }
        public string LeaveTypeEn { get; set; } = default!;
        public string LeaveTypeVi { get; set; } = default!;
        public string LeaveTypeJa { get; set; } = default!;
        public short TypeFlag { get; set; }
        public DateTime DateOfRequest { get; set; }
        public DateTime FromTime { get; set; }
        public DateTime ToTime { get; set; }
        public double TotalDay { get; set; }
        public string TotalHour { get; set; } = default!;
        public short Status { get; set; }
    }
}
