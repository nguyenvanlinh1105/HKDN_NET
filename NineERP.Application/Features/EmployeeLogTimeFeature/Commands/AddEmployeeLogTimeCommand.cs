using MediatR;
using Microsoft.EntityFrameworkCore;
using NineERP.Application.Constants.Messages;
using NineERP.Application.Interfaces.Common;
using NineERP.Application.Interfaces.Persistence;
using NineERP.Application.Wrapper;
using NineERP.Domain.Entities.Dat;
using static NineERP.Application.Constants.Global.GlobalConstants;

namespace NineERP.Application.Features.EmployeeLogTimeFeature.Commands
{
    public class AddEmployeeLogTimeCommand : IRequest<GenericResponse<object>>
    {
        public string Type { get; set; } = default!;

        public class Handler(IDateTimeService dateTimeService, ICurrentUserService currentUserService, IApplicationDbContext context) : IRequestHandler<AddEmployeeLogTimeCommand, GenericResponse<object>>
        {
            public async Task<GenericResponse<object>> Handle(AddEmployeeLogTimeCommand request, CancellationToken cancellationToken)
            {
                DateTime tstTime = TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.Local, dateTimeService.TimeZoneInfo);
                if (request.Type.Equals(TimeLogType.CheckIn.ToString()))
                {
                    var logTimeToday = await context.DatEmployeeLogTimes.FirstOrDefaultAsync(x => x.EmployeeNo == currentUserService.EmployeeNo && x.WorkDay.Date == tstTime.Date && !x.IsDeleted, cancellationToken: cancellationToken);
                    if (logTimeToday != null)
                        return GenericResponse<object>.ErrorResponse(409, ErrorMessages.GetMessage("ELT001"), "ELT001", ErrorMessages.GetMessage("ELT001"));

                    var addEmployeeLogTime = new DatEmployeeLogTime
                    {
                        EmployeeNo = currentUserService.EmployeeNo,
                        WorkDay = tstTime.Date,
                        CheckInOn = tstTime,
                        IsUpdate = false,
                        IsCheckLate = false, // TODO: Check late
                        IsCheckLeaveSoon = false
                    };

                    await context.DatEmployeeLogTimes.AddAsync(addEmployeeLogTime, cancellationToken);
                    await context.SaveChangesAsync(cancellationToken);
                }
                else if (request.Type.Equals(TimeLogType.CheckOut.ToString()))
                {
                    var logTimeToday = await context.DatEmployeeLogTimes.FirstOrDefaultAsync(x => x.EmployeeNo == currentUserService.EmployeeNo && x.WorkDay.Date == tstTime.Date && !x.IsDeleted, cancellationToken: cancellationToken);
                    if (logTimeToday == null)
                        return GenericResponse<object>.ErrorResponse(412, ErrorMessages.GetMessage("ELT002"), "ELT002", ErrorMessages.GetMessage("ELT002"));

                    if (logTimeToday is { CheckInOn: not null, CheckOutOn: null })
                    {
                        logTimeToday.CheckOutOn = tstTime;
                        context.DatEmployeeLogTimes.Update(logTimeToday);
                        await context.SaveChangesAsync(cancellationToken);
                    }
                    else
                    {
                        return GenericResponse<object>.ErrorResponse(409, ErrorMessages.GetMessage("ELT003"), "ELT003", ErrorMessages.GetMessage("ELT003"));
                    }
                }
                else
                {
                    return GenericResponse<object>.ErrorResponse(400, ErrorMessages.GetMessage("ELT004"), "ELT004", ErrorMessages.GetMessage("ELT004"));
                }

                return GenericResponse<object>.SuccessResponse(200, "", new {dateTime = tstTime});
            }
        }
    }
}
