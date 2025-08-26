using MediatR;
using Microsoft.EntityFrameworkCore;
using NineERP.Application.Dtos.AuditLog;
using NineERP.Application.Interfaces.Persistence;
using NineERP.Application.Request;
using NineERP.Application.Wrapper;
using System.Linq.Dynamic.Core;

namespace NineERP.Application.Features.AuditLogsFeature.Queries;

public record GetAuditLogsPaginationQuery(AuditLogRequest Request) : IRequest<PaginatedResult<AuditLogDto>>
{
    public class Handler(IApplicationDbContext context)
        : IRequestHandler<GetAuditLogsPaginationQuery, PaginatedResult<AuditLogDto>>
    {
        public async Task<PaginatedResult<AuditLogDto>> Handle(GetAuditLogsPaginationQuery request, CancellationToken cancellationToken)
        {
            var keyword = request.Request.Keyword?.ToLower();

            var query = context.AuditLogs.AsNoTracking()
                .Where(x => !x.IsDeleted &&
                            (string.IsNullOrEmpty(keyword) ||
                             x.TableName.ToLower().Contains(keyword) ||
                             x.ActionType.ToLower().Contains(keyword) ||
                             x.UserName!.ToLower().Contains(keyword)));

            var totalRecords = await query.CountAsync(cancellationToken);

            var result = await query.OrderBy(request.Request.OrderBy)
                .Skip((request.Request.PageNumber - 1) * request.Request.PageSize)
                .Take(request.Request.PageSize)
                .Select(x => new AuditLogDto
                {
                    Id = x.Id,
                    TableName = x.TableName,
                    UserId = x.UserId,
                    UserName = x.UserName,
                    ActionType = x.ActionType,
                    ActionTimestamp = x.ActionTimestamp,
                    KeyValues = x.KeyValues,
                    OldValues = x.OldValues,
                    NewValues = x.NewValues,
                    IpAddress = x.IpAddress
                })
                .ToListAsync(cancellationToken);

            return PaginatedResult<AuditLogDto>.Success(result, totalRecords, request.Request.PageNumber, request.Request.PageSize);
        }
    }
}
