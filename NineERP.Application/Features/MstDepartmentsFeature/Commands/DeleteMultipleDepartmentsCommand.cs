using MediatR;
using Microsoft.EntityFrameworkCore;
using NineERP.Application.Constants.Messages;
using NineERP.Application.Interfaces.Common;
using NineERP.Application.Interfaces.Persistence;
using NineERP.Application.Wrapper;
using NineERP.Domain.Entities.Mst;
using NineERP.Application.Dtos.AuditLog;

namespace NineERP.Application.Features.MstDepartmentsFeature.Commands;

public record DeleteMultipleDepartmentsCommand(List<int> Ids) : IRequest<IResult>;

public class DeleteMultipleDepartmentsCommandHandler : IRequestHandler<DeleteMultipleDepartmentsCommand, IResult>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IAuditLogService _auditLog;

    public DeleteMultipleDepartmentsCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser,
        IAuditLogService auditLog)
    {
        _context = context;
        _currentUser = currentUser;
        _auditLog = auditLog;
    }

    public async Task<IResult> Handle(DeleteMultipleDepartmentsCommand request, CancellationToken cancellationToken)
    {
        var departments = await _context.MstDepartments
            .Where(x => request.Ids.Contains(x.Id) && !x.IsDeleted)
            .ToListAsync(cancellationToken);

        if (!departments.Any())
            return await Result.FailAsync(MessageConstants.NotFound);

        var now = DateTime.Now;
        var userId = _currentUser.UserId ?? "system";
        var userName = _currentUser.UserName ?? "unknown";
        var ipAddress = _currentUser.IpAddress;

        foreach (var department in departments)
        {
            department.IsDeleted = true;
            department.LastModifiedOn = now;
            department.LastModifiedBy = userId;
            _context.MstDepartments.Update(department);
        }

        await _context.SaveChangesAsync(cancellationToken);

        await _auditLog.LogAsync(new AuditLogDto
        {
            TableName = "MstDepartments",
            ActionType = "DeleteMultiple",
            UserId = userId,
            UserName = userName,
            ActionTimestamp = now,
            KeyValues = string.Join(",", request.Ids),
            OldValues = null,
            NewValues = null,
            IpAddress = ipAddress
        }, cancellationToken);

        return await Result.SuccessAsync(MessageConstants.DeleteSuccess);
    }
}
