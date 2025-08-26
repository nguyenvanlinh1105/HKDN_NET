using MediatR;
using Microsoft.EntityFrameworkCore;
using NineERP.Application.Constants.Employee;
using NineERP.Application.Dtos.Attendance;
using NineERP.Application.Interfaces.Common;
using NineERP.Application.Interfaces.Persistence;
using NineERP.Application.Wrapper;

namespace NineERP.Application.Features.EmployeeLogTimeFeature.Queries.GetHistoryAttendanceCurrentUserQuery
{
    public record GetHistoryLogTimeCurrentUserQuery : IRequest<GenericResponse<HistoryAttendancesCurrentUserDto>>
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public class Handler(
            IApplicationDbContext context,
            ICurrentUserService currentUserService, IDateTimeService dateTimeService)
            : IRequestHandler<GetHistoryLogTimeCurrentUserQuery, GenericResponse<HistoryAttendancesCurrentUserDto>>
        {
            public async Task<GenericResponse<HistoryAttendancesCurrentUserDto>> Handle(GetHistoryLogTimeCurrentUserQuery request, CancellationToken cancellationToken)
            {
                // Get Leaves
                var leaves = await context.DatEmployeeLeaves.AsNoTracking()
                    .Where(el => el.FromTime.Date <= request.StartDate.Date
                              && el.ToTime.Date >= request.EndDate.Date
                              && el.EmployeeNo.Equals(currentUserService.EmployeeNo)
                              && !el.IsDeleted)
                    .Join(context.KbnLeaveTypes.AsNoTracking(),
                        el => el.LeaveTypeId,
                        lt => lt.Id,
                        (el, lt) => new { EmployeeLeave = el, LeaveType = lt })
                    .Select(x => new
                    {
                        x.EmployeeLeave.Id,
                        x.EmployeeLeave.FromTime,
                        x.EmployeeLeave.ToTime,
                        LeaveTypeEn = x.LeaveType.NameEn,
                        LeaveTypeJa = x.LeaveType.NameJp,
                        LeaveTypeVi = x.LeaveType.NameVi,
                        x.EmployeeLeave.Status,
                    })
                    .ToListAsync(cancellationToken);

                // Get holidays
                var holidays = await context.DatCalendarHolidays.AsNoTracking()
                    .Where(ch => ch.Year >= request.StartDate.Year
                              && ch.Year <= request.EndDate.Year
                              && !ch.IsDeleted)
                    .Join(context.KbnHolidayTypes.AsNoTracking(),
                        ch => ch.HolidayTypeId,
                        ht => ht.Id,
                        (ch, ht) => new
                        {
                            Title = ch.Description,
                            Date = new DateTime(ch.Year, ch.Month, ch.Day),
                            TypeEn = ht.NameEn,
                            TypeVi = ht.NameVi,
                            TypeJa = ht.NameJp,
                        })
                    .ToListAsync(cancellationToken);

                // Get log time
                var logTimes = await context.DatEmployeeLogTimes.AsNoTracking()
                        .Where(elt => elt.WorkDay.Date >= request.StartDate.Date
                                   && elt.WorkDay.Date <= request.EndDate.Date
                                   && elt.EmployeeNo.Equals(currentUserService.EmployeeNo)
                                   && !elt.IsDeleted)
                        .Select(elt => new
                        {
                            elt.Id,
                            elt.WorkDay,
                            elt.CheckInOn,
                            elt.CheckOutOn,
                            elt.Note,
                            IsLate = elt.IsCheckLate ?? false,
                            IsLeaveSoon = elt.IsCheckLeaveSoon ?? false
                        })
                    .ToListAsync(cancellationToken);

                // Combine data
                var resultData = new List<HistoryAttendanceCurrentUserDto>();
                for (var date = request.StartDate.Date; date <= request.EndDate.Date; date = date.AddDays(1))
                {
                    var logTime = logTimes.FirstOrDefault(lt => lt.WorkDay.Date == date.Date);
                    var leave = leaves.Where(x => x.FromTime.Date <= date.Date && x.ToTime.Date >= date.Date).ToList();
                    var holiday = holidays.Where(x => x.Date.Date == date.Date).ToList();
                    var historyAttendance = new HistoryAttendanceCurrentUserDto
                    {
                        Id = logTime?.Id ?? 0,
                        WorkDay = date.Date,
                        CheckInOn = logTime?.CheckInOn,
                        CheckOutOn = logTime?.CheckOutOn,
                        IsLate = logTime?.IsLate ?? false,
                        IsLeaveSoon = logTime?.IsLeaveSoon ?? false,
                        Note = logTime?.Note,
                        LeaveTypes = leave.Select(l => new LeaveApplicationDto
                        {
                            Id = l.Id,
                            LeaveTypeEn = l.LeaveTypeEn,
                            LeaveTypeJa = l.LeaveTypeJa,
                            LeaveTypeVi = l.LeaveTypeVi,
                            Status = l.Status
                        }).ToList(),
                        AnnualCalendarHolidays = holiday.Select(h => new AnnualCalendarHolidayDto
                        {
                            Title = h.Title,
                            TypeEn = h.TypeEn,
                            TypeVi = h.TypeVi,
                            TypeJa = h.TypeJa,
                        }).ToList()
                    };
                    resultData.Add(historyAttendance);
                }
                // Get count of modification
                var modificationCount = await context.DatEmployeeLogTimes.AsNoTracking()
                    .Where(x => x.EmployeeNo.Equals(currentUserService.EmployeeNo)
                              && x.WorkDay.Month == dateTimeService.Now.Month
                              && x.IsUpdate == true
                              && !x.IsDeleted)
                    .CountAsync(cancellationToken);

                // Pagination
                var paginatedData = resultData
                    .OrderByDescending(x => x.WorkDay)
                    .Skip((request.PageNumber - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .ToList();
                var totalRecords = resultData.Count;
                var totalPages = (int)Math.Ceiling((double)totalRecords / request.PageSize);

                // Create response
                var response = new HistoryAttendancesCurrentUserDto
                {
                    HistoryAttendanceCurrentUser = paginatedData,
                    TotalCount = totalRecords,
                    TotalPages = totalPages,
                    CurrentPage = request.PageNumber,
                    PageSize = request.PageSize,
                    ModificationAttendanceCount = (int)StaticVariable.NumberOfUpdateEmployeeLogTime.Number - modificationCount
                };

                return GenericResponse<HistoryAttendancesCurrentUserDto>.SuccessResponse(200, string.Empty, response);
            }
        }
    }
}
