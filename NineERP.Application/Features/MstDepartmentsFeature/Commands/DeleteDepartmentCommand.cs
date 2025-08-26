using MediatR;
using Microsoft.EntityFrameworkCore;
using NineERP.Application.Constants.Messages;
using NineERP.Application.Dtos.AuditLog;
using NineERP.Application.Interfaces.Common;
using NineERP.Application.Interfaces.Persistence;
using NineERP.Application.Wrapper;
using NineERP.Domain.Entities.Mst;
using System.Text.Json;

namespace NineERP.Application.Features.MstDepartmentsFeature.Commands;

public record DeleteDepartmentCommand(int Id) : IRequest<IResult>;

public class DeleteDepartmentCommandHandler : IRequestHandler<DeleteDepartmentCommand, IResult>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IAuditLogService _auditLog;

    public DeleteDepartmentCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser,
        IAuditLogService auditLog)
    {
        _context = context;
        _currentUser = currentUser;
        _auditLog = auditLog;
    }

    public async Task<IResult> Handle(DeleteDepartmentCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.MstDepartments
            .AsTracking()
            .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, cancellationToken);

        if (entity == null)
            return await Result.FailAsync(MessageConstants.NotFound);

        var now = DateTime.Now;
        var userId = _currentUser.UserId ?? "system";
        var userName = _currentUser.UserName ?? "unknown";
        var ipAddress = _currentUser.IpAddress;

        var oldValuesJson = JsonSerializer.Serialize(entity);

        entity.IsDeleted = true;
        entity.LastModifiedOn = now;
        entity.LastModifiedBy = userId;

        _context.MstDepartments.Update(entity);
        await _context.SaveChangesAsync(cancellationToken);

        await _auditLog.LogAsync(new AuditLogDto
        {
            TableName = "MstDepartments",
            ActionType = "Delete",
            UserId = userId,
            UserName = userName,
            ActionTimestamp = now,
            KeyValues = entity.Id.ToString(),
            OldValues = oldValuesJson,
            NewValues = null,
            IpAddress = ipAddress
        }, cancellationToken);

        return await Result.SuccessAsync(MessageConstants.DeleteSuccess);
    }
}
