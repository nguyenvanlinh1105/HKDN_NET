using MediatR;
using Microsoft.EntityFrameworkCore;
using NineERP.Application.Dtos.Attendance;
using NineERP.Application.Interfaces.Common;
using NineERP.Application.Interfaces.Persistence;
using NineERP.Application.Wrapper;

namespace NineERP.Application.Features.EmployeeLogTimeFeature.Queries.GetStatusQuery
{
    public record GetStatusQuery : IRequest<GenericResponse<AttendanceStatusDto>>
    {
        public class Handler(IApplicationDbContext context, IDateTimeService dateTimeService, ICurrentUserService currentUserService) : IRequestHandler<GetStatusQuery, GenericResponse<AttendanceStatusDto>>
        {
            public async Task<GenericResponse<AttendanceStatusDto>> Handle(GetStatusQuery request, CancellationToken cancellationToken)
            {
                DateTime tstTime = TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.Local, dateTimeService.TimeZoneInfo);
                var result = new AttendanceStatusDto
                {
                    CheckIn = false,
                    CheckOut = false,
                    CheckInDateTime = null,
                    CheckOutDateTime = null,
                };

                var logTimeToday = await context.DatEmployeeLogTimes.AsNoTracking().FirstOrDefaultAsync(x => x.EmployeeNo == currentUserService.EmployeeNo && x.WorkDay.Date == tstTime.Date && !x.IsDeleted, cancellationToken: cancellationToken);

                if (logTimeToday == null)
                    return GenericResponse<AttendanceStatusDto>.SuccessResponse(200, string.Empty, result);
                
                if (logTimeToday is { CheckInOn: not null, CheckOutOn: not null })
                {
                    result.CheckIn = true;
                    result.CheckOut = true;
                    result.CheckInDateTime = logTimeToday.CheckInOn;
                    result.CheckOutDateTime = logTimeToday.CheckOutOn;
                    return GenericResponse<AttendanceStatusDto>.SuccessResponse(200, string.Empty, result);
                }

                result.CheckIn = true;
                result.CheckInDateTime = logTimeToday.CheckInOn;
                return GenericResponse<AttendanceStatusDto>.SuccessResponse(200, string.Empty, result);
            }
        }
    }
}
