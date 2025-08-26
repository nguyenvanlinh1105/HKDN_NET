using NineERP.Application.Dtos.AuditLog;

namespace NineERP.Application.Interfaces.Common
{
    public interface IAuditLogService
    {
        Task LogAsync(AuditLogDto dto, CancellationToken cancellationToken = default);
    }

}
