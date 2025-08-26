using Microsoft.AspNetCore.Http;
using NineERP.Application.Dtos.AuditLog;
using NineERP.Application.Interfaces.Common;
using NineERP.Domain.Entities;
using NineERP.Infrastructure.Contexts;

namespace NineERP.Infrastructure.Services.Common;

public class AuditLogService : IAuditLogService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuditLogService(ApplicationDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    {
        _dbContext = dbContext;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task LogAsync(AuditLogDto dto, CancellationToken cancellationToken = default)
    {
        var entity = new AuditLog
        {
            TableName = dto.TableName,
            UserId = dto.UserId,
            UserName = dto.UserName,
            ActionType = dto.ActionType,
            ActionTimestamp = dto.ActionTimestamp,
            KeyValues = dto.KeyValues,
            OldValues = dto.OldValues,
            NewValues = dto.NewValues,
            IpAddress = dto.IpAddress ?? _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString()
        };

        _dbContext.AuditLogs.Add(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
