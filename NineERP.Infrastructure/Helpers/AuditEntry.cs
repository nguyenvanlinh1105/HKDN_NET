using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Text.Json;
using NineERP.Domain.Entities;

namespace NineERP.Infrastructure.Helpers
{
    public class AuditEntry
    {
        public AuditEntry(EntityEntry entry)
        {
            Entry = entry;
        }

        public EntityEntry Entry { get; }

        public string TableName { get; set; } = string.Empty;

        public Dictionary<string, object?> KeyValues { get; } = new();
        public Dictionary<string, object?> OldValues { get; } = new();
        public Dictionary<string, object?> NewValues { get; } = new();
        public string ActionType { get; set; } = string.Empty;

        public AuditLog ToAuditLog(string userId, string userName, string? ip = null)
        {
            return new AuditLog
            {
                TableName = TableName,
                UserId = userId,
                UserName = userName,
                ActionType = ActionType,
                ActionTimestamp = DateTime.UtcNow,
                KeyValues = JsonSerializer.Serialize(KeyValues),
                OldValues = JsonSerializer.Serialize(OldValues),
                NewValues = JsonSerializer.Serialize(NewValues),
                IpAddress = ip,
                CreatedOn = DateTime.UtcNow,
                CreatedBy = string.IsNullOrWhiteSpace(userName) ? "SYSTEM" : userName,
                IsDeleted = false
            };
        }
    }
}
