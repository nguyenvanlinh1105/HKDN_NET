using MediatR;
using Microsoft.EntityFrameworkCore;
using NineERP.Application.Constants.Employee;
using NineERP.Application.Constants.Messages;
using NineERP.Application.Extensions;
using NineERP.Application.Interfaces.Common;
using NineERP.Application.Interfaces.LeaveCalculation;
using NineERP.Application.Interfaces.Persistence;
using NineERP.Application.Wrapper;
using NineERP.Domain.Entities.Dat;

namespace NineERP.Application.Features.MyLeaveFeature.Commands
{
    /// <summary>
    /// Command to add a new employee leave request
    /// </summary>
    public class AddEmployeeLeaveCommand : IRequest<GenericResponse<object>>
    {
        public string Reason { get; set; } = default!;
        public short LeaveTypeId { get; set; }
        public DateTime FromTime { get; set; }
        public DateTime ToTime { get; set; }

        public class Handler(IApplicationDbContext context, IDateTimeService dateTimeService, ICurrentUserService currentUserService, ILeaveCalculationService leaveCalculationService) : IRequestHandler<AddEmployeeLeaveCommand, GenericResponse<object>>
        {
            /// <summary>
            /// Main handler method for processing leave requests
            /// </summary>
            /// <param name="request">Leave request details</param>
            /// <param name="cancellationToken">Cancellation token</param>
            /// <returns>Response with operation status</returns>
            public async Task<GenericResponse<object>> Handle(AddEmployeeLeaveCommand request, CancellationToken cancellationToken)
            {
                // Convert and normalize time values from request
                var convertedTimes = ConvertAndNormalizeTimes(request);
                var fromTime = convertedTimes.FromTime;
                var toTime = convertedTimes.ToTime;

                // Validate the leave request against business rules
                await ValidateLeaveRequest(fromTime, toTime, request.LeaveTypeId, cancellationToken);

                // Calculate total days and hours for the leave period
                var totalDayLeave = await leaveCalculationService.CalculateTotalDayLeave(
                    fromTime, toTime, currentUserService.EmployeeNo, cancellationToken);

                // Start transaction to ensure data consistency
                using var transaction = await context.BeginTransactionAsync(cancellationToken);
                try
                {
                    // Get leave type for further processing
                    var leaveType = await context.KbnLeaveTypes
                        .AsNoTracking()
                        .FirstOrDefaultAsync(x => x.Id == request.LeaveTypeId && !x.IsDeleted, cancellationToken);

                    // Process based on leave type (annual leave vs. other types)
                    if (leaveType!.LeaveTypeFlag == (short)StaticVariable.MyLeaveType.ANNUAL_LEAVE)
                    {
                        await ProcessAnnualLeave(request, fromTime, toTime, totalDayLeave, cancellationToken);
                    }
                    else
                    {
                        await ProcessOtherLeaveType(request, fromTime, toTime, totalDayLeave, cancellationToken);
                    }

                    // Commit all changes if successful
                    await transaction.CommitAsync(cancellationToken);
                    return GenericResponse<object>.SuccessResponse(200, ErrorMessages.GetMessage("SYS0003"), null!);
                }
                catch (Exception ex)
                {
                    // Rollback all changes if any operation fails
                    await transaction.RollbackAsync(cancellationToken);
                    throw new ApiException($"Failed to process leave request: {ex.Message}");
                }
            }

            /// <summary>
            /// Converts request times to appropriate timezone and normalizes them
            /// </summary>
            private (DateTime FromTime, DateTime ToTime) ConvertAndNormalizeTimes(AddEmployeeLeaveCommand request)
            {
                // Convert times to the correct timezone based on system configuration
                var fromTime = TimeZoneInfo.ConvertTime(request.FromTime.ToLocalTime(), TimeZoneInfo.Local, dateTimeService.TimeZoneInfo);
                var toTime = TimeZoneInfo.ConvertTime(request.ToTime.ToLocalTime(), TimeZoneInfo.Local, dateTimeService.TimeZoneInfo);

                // Normalize times by removing seconds for consistency
                return (
                    FromTime: new DateTime(fromTime.Year, fromTime.Month, fromTime.Day, fromTime.Hour, fromTime.Minute, 0),
                    ToTime: new DateTime(toTime.Year, toTime.Month, toTime.Day, toTime.Hour, toTime.Minute, 0)
                );
            }

            /// <summary>
            /// Validates the leave request against business rules
            /// </summary>
            private async Task ValidateLeaveRequest(DateTime fromTime, DateTime toTime, short leaveTypeId, CancellationToken cancellationToken)
            {
                // Check if a leave request already exists for this period
                var isExisted = await context.DatEmployeeLeaves
                    .AsNoTracking()
                    .AnyAsync(x => x.Status != (short)StaticVariable.ApproveStatus.Declined
                            && x.Status != (short)StaticVariable.ApproveStatus.Cancel
                            && x.EmployeeNo == currentUserService.EmployeeNo
                            && x.ToTime > fromTime
                            && x.FromTime < toTime
                            && !x.IsDeleted,
                        cancellationToken: cancellationToken);

                if (isExisted)
                    throw new ApiException("NP011E", "Leave application already exists");

                // Check if the requested leave period includes a holiday
                var isHoliday = await context.DatCalendarHolidays
                    .AsNoTracking()
                    .AnyAsync(x => (x.Year == fromTime.Year && x.Month == fromTime.Month && x.Day == fromTime.Day)
                                || (x.Year == toTime.Year && x.Month == toTime.Month && x.Day == toTime.Day),
                            cancellationToken);

                if (isHoliday)
                    throw new ApiException("NP028E", "You can't create a holiday, because it's a public holiday");

                // Verify the leave type exists
                var leaveType = await context.KbnLeaveTypes
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.Id == leaveTypeId && !x.IsDeleted, cancellationToken);

                if (leaveType == null)
                    throw new ApiException("NP001E", "Leave Type not found.");
            }

            /// <summary>
            /// Processes an annual leave request including balance checking
            /// </summary>
            private async Task ProcessAnnualLeave(
                AddEmployeeLeaveCommand request,
                DateTime fromTime,
                DateTime toTime,
                (float TotalDay, double TotalHour, string TotalHourText, List<(DateTime Date, bool IsHolidayAndSaturdayOrSunday, float LeaveHours)> DateLeaves) totalDayLeave,
                CancellationToken cancellationToken)
            {
                // Check if employee has sufficient annual leave balance
                var employeeAnnualLeave = await context.DatEmployeeAnnualLeaves
                    .FirstOrDefaultAsync(x => x.EmployeeNo == currentUserService.EmployeeNo && !x.IsDeleted, cancellationToken);

                if (employeeAnnualLeave == null)
                    throw new ApiException("NP002E", "Annual leave has expired. Please choose another type of leave");

                // Calculate remaining leave days after this request
                var remainingDay = (employeeAnnualLeave.LeaveCurrentYear + employeeAnnualLeave.LeaveLastYear) -
                                    employeeAnnualLeave.LeaveUsed - (decimal)totalDayLeave.TotalDay;

                if (remainingDay < 0)
                    throw new ApiException("NP001E", "Annual leave has expired. Please choose another type of leave");

                // Create the leave record in database
                var addLeave = await CreateLeaveRecord(request, fromTime, toTime, totalDayLeave, cancellationToken);

                // Create approval records for this leave request
                await CreateLeaveApprovals(addLeave.Id, cancellationToken);

                // Update employee's annual leave balance
                employeeAnnualLeave.LeaveUsed += (decimal)totalDayLeave.TotalDay;
                context.DatEmployeeAnnualLeaves.Update(employeeAnnualLeave);

                // Create detailed leave records by month
                await CreateLeaveDetails(addLeave.Id, fromTime, toTime, totalDayLeave, cancellationToken);

                // Save all changes
                await context.SaveChangesAsync(cancellationToken);
            }

            /// <summary>
            /// Processes non-annual leave requests (no balance checking)
            /// </summary>
            private async Task ProcessOtherLeaveType(
                AddEmployeeLeaveCommand request,
                DateTime fromTime,
                DateTime toTime,
                (float TotalDay, double TotalHour, string TotalHourText, List<(DateTime Date, bool IsHolidayAndSaturdayOrSunday, float LeaveHours)> DateLeaves) totalDayLeave,
                CancellationToken cancellationToken)
            {
                // Create leave record, approvals and details
                var addLeave = await CreateLeaveRecord(request, fromTime, toTime, totalDayLeave, cancellationToken);
                await CreateLeaveApprovals(addLeave.Id, cancellationToken);
                await CreateLeaveDetails(addLeave.Id, fromTime, toTime, totalDayLeave, cancellationToken);

                await context.SaveChangesAsync(cancellationToken);
            }

            /// <summary>
            /// Creates the main leave record in the database
            /// </summary>
            private async Task<DatEmployeeLeave> CreateLeaveRecord(
                AddEmployeeLeaveCommand request,
                DateTime fromTime,
                DateTime toTime,
                (float TotalDay, double TotalHour, string TotalHourText, List<(DateTime Date, bool IsHolidayAndSaturdayOrSunday, float LeaveHours)> DateLeaves) totalDayLeave,
                CancellationToken cancellationToken)
            {
                // Initialize new employee leave entity
                var addLeave = new DatEmployeeLeave
                {
                    EmployeeNo = currentUserService.EmployeeNo,
                    Reason = request.Reason,
                    LeaveTypeId = request.LeaveTypeId,
                    Status = (short)StaticVariable.ApproveStatus.New, // Initial status is New
                    FromTime = fromTime,
                    ToTime = toTime,
                    TotalDay = (decimal)totalDayLeave.TotalDay,
                    TotalHour = totalDayLeave.TotalHourText,
                    TotalTime = totalDayLeave.TotalHour
                };

                // Add to context and save to get generated ID
                await context.DatEmployeeLeaves.AddAsync(addLeave);
                await context.SaveChangesAsync(cancellationToken);

                return addLeave;
            }

            /// <summary>
            /// Creates approval records for the leave request
            /// </summary>
            private async Task CreateLeaveApprovals(long employeeLeaveId, CancellationToken cancellationToken)
            {
                // Get approvers for the current employee
                var employeeApproves = await context.DatEmployeeApproves
                    .Where(x => x.EmployeeNoLeave == currentUserService.EmployeeNo && !x.IsDeleted)
                    .ToListAsync(cancellationToken);

                if (employeeApproves.Any())
                {
                    // Create approval records for each approver
                    var leaveApproves = employeeApproves.Select(x => new DatLeaveApprove
                    {
                        EmployeeLeaveId = employeeLeaveId,
                        EmployeeApproveId = x.Id,
                        EmployeeNoApprove = x.EmployeeNoApprove,
                        StatusApprove = (short)StaticVariable.ApproveStatus.Waiting, // Initial status is Waiting
                        CreatedBy = currentUserService.UserId,
                        ApproveLevel = x.ApproveLevel,
                        ApproveType = x.ApproveType,
                        DateApprove = null // No approval date yet
                    }).ToList();

                    await context.DatLeaveApproves.AddRangeAsync(leaveApproves, cancellationToken);
                }
            }

            /// <summary>
            /// Creates detailed leave records by month
            /// </summary>
            /// <param name="employeeLeaveId"></param>
            /// <param name="fromTime"></param>
            /// <param name="toTime"></param>
            /// <param name="totalDayLeave"></param>
            /// <param name="cancellationToken"></param>
            /// <returns></returns>
            private async Task CreateLeaveDetails(
                long employeeLeaveId,
                DateTime fromTime,
                DateTime toTime,
                (float TotalDay, double TotalHour, string TotalHourText, List<(DateTime Date, bool IsHolidayAndSaturdayOrSunday, float LeaveHours)> DateLeaves) totalDayLeave,
                CancellationToken cancellationToken)
            {
                // Check if leave spans multiple months
                if (fromTime.Month != toTime.Month)
                {
                    // Handle leave spanning multiple months by creating separate records for each month

                    // Get the last day of the first month
                    var endTimeInMonth = new DateTime(fromTime.Year, fromTime.Month, DateTime.DaysInMonth(fromTime.Year, fromTime.Month), 23, 59, 59);

                    // Get leave days in the first month
                    var dateLeaves = totalDayLeave.DateLeaves
                        .Where(x => x.Date >= fromTime && x.Date <= endTimeInMonth && !x.IsHolidayAndSaturdayOrSunday)
                        .ToList();

                    // Create record for the first month
                    var detailFromMonth = new DatEmployeeLeaveDetail
                    {
                        EmployeeLeaveId = employeeLeaveId,
                        EmployeeNo = currentUserService.EmployeeNo,
                        Month = fromTime.Month,
                        Year = fromTime.Year,
                        TotalDay = dateLeaves.Sum(x => x.LeaveHours),
                        CreatedBy = currentUserService.UserId
                    };

                    // Get the first day of the second month
                    var startTimeInMonth = new DateTime(toTime.Year, toTime.Month, 1, 0, 0, 0);

                    // Get leave days in the second month
                    var dateLeavesTo = totalDayLeave.DateLeaves
                        .Where(x => x.Date >= startTimeInMonth && x.Date <= toTime && !x.IsHolidayAndSaturdayOrSunday)
                        .ToList();

                    // Create record for the second month
                    var detailToMonth = new DatEmployeeLeaveDetail
                    {
                        EmployeeLeaveId = employeeLeaveId,
                        EmployeeNo = currentUserService.EmployeeNo,
                        Month = toTime.Month,
                        Year = toTime.Year,
                        TotalDay = dateLeavesTo.Sum(x => x.LeaveHours),
                        CreatedBy = currentUserService.UserId
                    };

                    // Add both month records
                    await context.DatEmployeeLeaveDetails.AddRangeAsync(detailFromMonth, detailToMonth);
                }
                else
                {
                    // Handle leave within a single month
                    var detail = new DatEmployeeLeaveDetail
                    {
                        EmployeeLeaveId = employeeLeaveId,
                        EmployeeNo = currentUserService.EmployeeNo,
                        Month = toTime.Month,
                        Year = toTime.Year,
                        TotalDay = totalDayLeave.TotalDay,
                        CreatedBy = currentUserService.UserId
                    };

                    await context.DatEmployeeLeaveDetails.AddAsync(detail);
                }
            }
        }
    }
}