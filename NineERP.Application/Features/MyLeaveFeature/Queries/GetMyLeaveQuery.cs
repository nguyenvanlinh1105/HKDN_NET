using MediatR;
using Microsoft.EntityFrameworkCore;
using NineERP.Application.Dtos.MyLeave;
using NineERP.Application.Interfaces.Common;
using NineERP.Application.Interfaces.Persistence;
using NineERP.Application.Wrapper;

namespace NineERP.Application.Features.MyLeaveFeature.Queries
{
    public record GetMyLeaveQuery : IRequest<GenericResponse<MyLeavesDto>>
    {
        public string? Keyword { get; set; }
        public List<short>? LeaveTypeId { get; set; }
        public List<short>? Status { get; set; }
        public DateTime? FromTime { get; set; }
        public DateTime? ToTime { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public class Handler(IApplicationDbContext context, ICurrentUserService currentUserService) : IRequestHandler<GetMyLeaveQuery, GenericResponse<MyLeavesDto>>
        {
            public async Task<GenericResponse<MyLeavesDto>> Handle(GetMyLeaveQuery request, CancellationToken cancellationToken)
            {
                var query = from employeeLeave in context.DatEmployeeLeaves.AsNoTracking()
                            join leaveType in context.KbnLeaveTypes.AsNoTracking()
                                on employeeLeave.LeaveTypeId equals leaveType.Id
                            where (!request.FromTime.HasValue || employeeLeave.FromTime.Date.CompareTo(request.FromTime.Value.Date) >= 0)
                            && (!request.ToTime.HasValue || employeeLeave.ToTime.Date.CompareTo(request.ToTime.Value.Date) <= 0)
                            && (request.LeaveTypeId == null || request.LeaveTypeId.Contains(employeeLeave.LeaveTypeId))
                            && (request.Status == null || request.Status.Contains(employeeLeave.Status))
                            && !employeeLeave.IsDeleted && !leaveType.IsDeleted
                            && employeeLeave.EmployeeNo == currentUserService.EmployeeNo
                            && (string.IsNullOrEmpty(request.Keyword) || employeeLeave.Reason.Contains(request.Keyword))
                            select new MyLeaveDto
                            {
                                Id = employeeLeave.Id,
                                Reason = employeeLeave.Reason,
                                LeaveTypeEn = leaveType.NameEn,
                                LeaveTypeVi = leaveType.NameVi,
                                LeaveTypeJa = leaveType.NameJp,
                                FromTime = employeeLeave.FromTime,
                                ToTime = employeeLeave.ToTime,
                                TotalDay = (double)employeeLeave.TotalDay,
                                TotalHour = employeeLeave.TotalHour,
                                Status = employeeLeave.Status,
                                DateOfRequest = employeeLeave.CreatedOn,
                                LeaveTypeId = employeeLeave.LeaveTypeId,
                                TypeFlag = leaveType.LeaveTypeFlag,
                            };

                var totalRecords = await query.CountAsync(cancellationToken);

                var pagedQuery = query
                    .OrderByDescending(t => t.Id)
                    .Skip((request.PageNumber - 1) * request.PageSize)
                    .Take(request.PageSize);

                var myLeaves = await pagedQuery.ToListAsync(cancellationToken);

                var result = new MyLeavesDto
                {
                    MyLeaves = myLeaves,
                    CurrentPage = request.PageNumber,
                    TotalPages = (int)Math.Ceiling((double)totalRecords / request.PageSize),
                    TotalCount = totalRecords,
                    PageSize = request.PageSize
                };

                return GenericResponse<MyLeavesDto>.SuccessResponse(200, string.Empty, result);
            }
        }
    }
}
