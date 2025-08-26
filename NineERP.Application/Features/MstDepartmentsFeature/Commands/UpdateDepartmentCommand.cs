using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NineERP.Application.Constants.Messages;
using NineERP.Application.Dtos.AuditLog;
using NineERP.Application.Dtos.MstDepartment;
using NineERP.Application.Interfaces.Common;
using NineERP.Application.Interfaces.Persistence;
using NineERP.Application.Wrapper;
using System.Text.Json;

namespace NineERP.Application.Features.MstDepartmentsFeature.Commands;

public record UpdateDepartmentCommand(DepartmentDetailDto Department) : IRequest<IResult>
{
    public class Handler(
        IApplicationDbContext context,
        IMapper mapper,
        ICurrentUserService currentUser,
        IAuditLogService auditLog) : IRequestHandler<UpdateDepartmentCommand, IResult>
    {
        public async Task<IResult> Handle(UpdateDepartmentCommand request, CancellationToken cancellationToken)
        {
            var department = await context.MstDepartments
                .AsTracking()
                .FirstOrDefaultAsync(x => x.Id == request.Department.Id && !x.IsDeleted, cancellationToken);

            if (department is null)
                return await Result.FailAsync(MessageConstants.NotFound);

            var now = DateTime.Now;
            var userId = currentUser.UserId ?? "system";
            var userName = currentUser.UserName ?? "unknown";
            var ipAddress = currentUser.IpAddress;

            var oldValues = JsonSerializer.Serialize(department);

            // Map updated values
            mapper.Map(request.Department, department);
            department.LastModifiedOn = now;
            department.LastModifiedBy = userId;

            context.MstDepartments.Update(department);
            await context.SaveChangesAsync(cancellationToken);

            var newValues = JsonSerializer.Serialize(department);

            await auditLog.LogAsync(new AuditLogDto
            {
                TableName = "MstDepartments",
                ActionType = "Update",
                UserId = userId,
                UserName = userName,
                ActionTimestamp = now,
                KeyValues = department.Id.ToString(),
                OldValues = oldValues,
                NewValues = newValues,
                IpAddress = ipAddress
            }, cancellationToken);

            return await Result.SuccessAsync(MessageConstants.UpdateSuccess);
        }
    }
}
