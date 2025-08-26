using AutoMapper;
using MediatR;
using NineERP.Application.Constants.Messages;
using NineERP.Application.Dtos.AuditLog;
using NineERP.Application.Dtos.MstDepartment;
using NineERP.Application.Interfaces.Common;
using NineERP.Application.Interfaces.Persistence;
using NineERP.Application.Wrapper;
using NineERP.Domain.Entities.Mst;

namespace NineERP.Application.Features.MstDepartmentsFeature.Commands;

public record CreateDepartmentCommand(DepartmentDetailDto Department) : IRequest<IResult>
{
    public class Handler(
        IApplicationDbContext context,
        IMapper mapper,
        ICurrentUserService currentUser,
        IAuditLogService auditLog) : IRequestHandler<CreateDepartmentCommand, IResult>
    {
        public async Task<IResult> Handle(CreateDepartmentCommand request, CancellationToken cancellationToken)
        {
            await using var transaction = await context.BeginTransactionAsync(cancellationToken);

            try
            {
                var now = DateTime.Now;
                var userId = currentUser.UserId ?? "system";
                var userName = currentUser.UserName ?? "unknown";
                var ipAddress = currentUser.IpAddress;

                var department = mapper.Map<MstDepartment>(request.Department);
                department.CreatedOn = now;
                department.CreatedBy = userId;
                department.LastModifiedOn = now;
                department.LastModifiedBy = userId;

                context.MstDepartments.Add(department);
                await context.SaveChangesAsync(cancellationToken);

                await auditLog.LogAsync(new AuditLogDto
                {
                    TableName = "MstDepartments",
                    ActionType = "Create",
                    UserId = userId,
                    UserName = userName,
                    ActionTimestamp = now,
                    KeyValues = department.Id.ToString(),
                    OldValues = null,
                    NewValues = System.Text.Json.JsonSerializer.Serialize(department),
                    IpAddress = ipAddress
                }, cancellationToken);

                await transaction.CommitAsync(cancellationToken);
                return await Result.SuccessAsync(MessageConstants.AddSuccess);
            }
            catch (Exception)
            {
                await transaction.RollbackAsync(cancellationToken);
                return await Result.FailAsync(MessageConstants.AddFail);
            }
        }
    }
}
