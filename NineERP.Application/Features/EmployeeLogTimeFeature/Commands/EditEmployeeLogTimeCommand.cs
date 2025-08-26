using MediatR;
using Microsoft.EntityFrameworkCore;
using NineERP.Application.Constants.Employee;
using NineERP.Application.Constants.Messages;
using NineERP.Application.Interfaces.Common;
using NineERP.Application.Interfaces.Persistence;
using NineERP.Application.Wrapper;

namespace NineERP.Application.Features.EmployeeLogTimeFeature.Commands
{
    public class EditEmployeeLogTimeCommand : IRequest<GenericResponse<object>>
    {
        public long Id { get; set; }

        public class Handler(ICurrentUserService currentUserService, IApplicationDbContext context) : IRequestHandler<EditEmployeeLogTimeCommand, GenericResponse<object>>
        {
            public async Task<GenericResponse<object>> Handle(EditEmployeeLogTimeCommand request, CancellationToken cancellationToken)
            {
                // Get Leaves by current user
                var employeeLogTimes = await context.DatEmployeeLogTimes
                    .Where(x => x.Id == request.Id
                             && x.EmployeeNo == currentUserService.EmployeeNo
                             && !x.IsDeleted).FirstOrDefaultAsync(cancellationToken);
                if (employeeLogTimes == null)
                    return GenericResponse<object>.ErrorResponse(400, ErrorMessages.GetMessage("DATA001"), "DATA001", ErrorMessages.GetMessage("DATA001"));
                if (employeeLogTimes.IsUpdate == true)
                    return GenericResponse<object>.ErrorResponse(400, ErrorMessages.GetMessage("ELT005"), "ELT005", ErrorMessages.GetMessage("ELT005"));

                var monthUpdate = employeeLogTimes.WorkDay.Month;
                var yearUpdate = employeeLogTimes.WorkDay.Year;
                var dayUpdate = employeeLogTimes.WorkDay.Day;
                var numberOfUpdateInMonth = await context.DatEmployeeLogTimes.AsNoTracking()
                    .AsNoTracking()
                    .CountAsync(x => x.WorkDay.Year == yearUpdate
                                  && x.WorkDay.Month == monthUpdate
                                  && x.EmployeeNo == currentUserService.EmployeeNo
                                  && x.IsUpdate == true
                                  && !x.IsDeleted, cancellationToken);
                if (numberOfUpdateInMonth >= (int)StaticVariable.NumberOfUpdateEmployeeLogTime.Number)
                    return GenericResponse<object>.ErrorResponse(400, ErrorMessages.GetMessage("ELT006"), "ELT006", ErrorMessages.GetMessage("ELT006"));

                var oldCheckIn = employeeLogTimes.CheckInOn;
                var oldCheckOut = employeeLogTimes.CheckOutOn;
                var morningStart = new DateTime(yearUpdate, monthUpdate, dayUpdate, 8, 0, 0);
                var afternoonEnd = new DateTime(yearUpdate, monthUpdate, dayUpdate, 18, 0, 0);

                // Get employee working hours
                var shift = await (from ds in context.DatEmployeeShifts.AsNoTracking()
                                   join mstShift in context.MstShifts.AsNoTracking() on ds.ShiftId equals mstShift.Id into mstShiftGroup
                                   from mstShift in mstShiftGroup.DefaultIfEmpty()
                                   where ds.EmployeeNo == currentUserService.EmployeeNo && !ds.IsDeleted
                                   select new
                                   {
                                       mstShift.MorningStartTime,
                                       mstShift.AfternoonEndTime,
                                   }).FirstOrDefaultAsync(cancellationToken);
                if (shift != null)
                {
                    var timeMorningParts = shift.MorningStartTime.Split(':');
                    int hourMorning = int.Parse(timeMorningParts[0]);
                    int minuteMorning = int.Parse(timeMorningParts[1]);

                    var timeAfternoonParts = shift.AfternoonEndTime.Split(':');
                    int hourAfternoon = int.Parse(timeAfternoonParts[0]);
                    int minuteAfternoon = int.Parse(timeAfternoonParts[1]);

                    morningStart = new DateTime(yearUpdate, monthUpdate, dayUpdate, hourMorning, minuteMorning, 0);
                    afternoonEnd = new DateTime(yearUpdate, monthUpdate, dayUpdate, hourAfternoon, minuteAfternoon, 0);
                }

                employeeLogTimes.CheckInOn = morningStart;
                employeeLogTimes.CheckOutOn = afternoonEnd;
                employeeLogTimes.Note = $"Check in: {oldCheckIn?.ToString("HH:mm")} -> {morningStart:HH:mm}, " +
                                        $"Check Out: {(oldCheckOut.HasValue ? $"{oldCheckOut:HH:mm} -> {afternoonEnd:HH:mm}" : $"N/A -> {afternoonEnd:HH:mm}")}";

                context.DatEmployeeLogTimes.Update(employeeLogTimes);
                await context.SaveChangesAsync(cancellationToken);

                return GenericResponse<object>.SuccessResponse(200, ErrorMessages.GetMessage("SYS0001"), null!);
            }
        }
    }
}
