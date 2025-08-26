using MediatR;
using Microsoft.EntityFrameworkCore;
using NineERP.Application.Constants.Employee;
using NineERP.Application.Dtos.MyLeave;
using NineERP.Application.Interfaces.Common;
using NineERP.Application.Interfaces.Persistence;
using NineERP.Application.Wrapper;
using static NineERP.Application.Constants.Employee.StaticVariable;

namespace NineERP.Application.Features.MyLeaveFeature.Queries
{
    public record GetLeaveSummaryQuery : IRequest<GenericResponse<LeaveSummaryDto>>
    {
        public class Handler(IApplicationDbContext context, ICurrentUserService currentUserService) : IRequestHandler<GetLeaveSummaryQuery, GenericResponse<LeaveSummaryDto>>
        {
            public async Task<GenericResponse<LeaveSummaryDto>> Handle(GetLeaveSummaryQuery request, CancellationToken cancellationToken)
            {
                var result = new LeaveSummaryDto();
                var totalSpecialLeaveDays = await (from el in context.DatEmployeeLeaves.AsNoTracking()
                                                   join lt in context.KbnLeaveTypes.AsNoTracking() on el.LeaveTypeId equals lt.Id into leaveTypeGroup
                                                   from leaveType in leaveTypeGroup.DefaultIfEmpty()
                                                   where !el.IsDeleted
                                                       && el.EmployeeNo == currentUserService.EmployeeNo
                                                       && el.Status == (short)ApproveStatus.Accepted
                                                       && leaveType.LeaveTypeFlag == (short)MyLeaveType.UNPAID_LEAVE
                                                   select el.TotalDay
                                                ).SumAsync();

                var employeeAnnualLeave = await context.DatEmployeeAnnualLeaves.AsNoTracking().FirstOrDefaultAsync(x => x.EmployeeNo.Equals(currentUserService.EmployeeNo) && !x.IsDeleted, cancellationToken);
                if (employeeAnnualLeave != null)
                {
                    result.TotalLeaveCanUse = employeeAnnualLeave.LeaveCurrentYear;
                    result.TotalLeaveUsed = employeeAnnualLeave.LeaveUsed;
                }

                result.TotalSpecialLeaveDays = totalSpecialLeaveDays;
                result.TotalRemainingLeave = result.TotalLeaveCanUse - result.TotalLeaveUsed;

                // Get leave types
                var leaveTypes = await context.KbnLeaveTypes.AsNoTracking()
                    .Where(x => !x.IsDeleted)
                    .Select(x => new LeaveTypeDto
                    {
                        NameEn = x.NameEn,
                        NameVi = x.NameVi,
                        NameJa = x.NameJp,
                        Color = x.Color,
                        TypeFlag = x.LeaveTypeFlag,
                        Acronym = x.Acronym
                    })
                    .ToListAsync(cancellationToken);

                result.LeaveMasterData.LeaveTypes = leaveTypes;

                // Get employee shift
                var employeeShift = (from s in context.MstShifts.AsNoTracking()
                                     join es in context.DatEmployeeShifts.AsNoTracking()
                                                   on s.Id equals es.ShiftId into leftJoinEmployeeShift
                                     from eShift in leftJoinEmployeeShift.DefaultIfEmpty()
                                     where eShift.EmployeeNo.Equals(currentUserService.EmployeeNo) && !eShift.IsDeleted
                                     select new EmployeeShiftDto
                                     {
                                         MorningStartTime = s.MorningStartTime,
                                         MorningEndTime = s.MorningEndTime,
                                         AfternoonStartTime = s.AfternoonStartTime,
                                         AfternoonEndTime = s.AfternoonEndTime,
                                         TotalHour = s.TotalHour,
                                     }).FirstOrDefault();

                result.LeaveMasterData.EmployeeShift = employeeShift;

                // Get holiday
                var holidays = await context.DatCalendarHolidays.AsNoTracking().Where(x => !x.IsDeleted).Select(x => new HolidayDto()
                {
                    TimeOfDay = x.TimeOfDay,
                    Title = x.Description,
                    Date = new DateTime(x.Year,x.Month, x.Day),
                }).ToListAsync();
                result.LeaveMasterData.Holidays = holidays;

                // Get employee approve
                var employeeApprove = await (from ea in context.DatEmployeeApproves.AsNoTracking()
                                       join employee in context.DatEmployees.AsNoTracking()
                                            on ea.EmployeeNoApprove equals employee.EmployeeNo
                                       where ea.ApproveType == (short)StaticVariable.ApproveType.Approve && !ea.IsDeleted
                                      && ea.EmployeeNoLeave == currentUserService.EmployeeNo
                                       select new EmployeeApproveDto()
                                       {
                                           ApproveLevel = ea.ApproveLevel,
                                           ApproveType = ea.ApproveType,
                                           EmployeeNoApprove = ea.EmployeeNoApprove,
                                           EmployeeApproveName = employee.NickName,
                                       }).ToListAsync(cancellationToken);
                result.LeaveMasterData.EmployeeApproves = employeeApprove;

                return GenericResponse<LeaveSummaryDto>.SuccessResponse(200, string.Empty, result);
            }
        }
    }
}
