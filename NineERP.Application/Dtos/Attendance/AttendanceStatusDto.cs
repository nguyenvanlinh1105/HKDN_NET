namespace NineERP.Application.Dtos.Attendance
{
    public class AttendanceStatusDto
    {
        public bool CheckIn { get; set; }
        public bool CheckOut { get; set; }
        public DateTime? CheckInDateTime { get; set; }
        public DateTime? CheckOutDateTime { get; set; }
    }
}
