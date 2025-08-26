namespace NineERP.Application.Dtos.AuditLog;

public class AuditLogDto
{
    public int Id { get; set; }
    public string TableName { get; set; } = default!;
    public string? UserId { get; set; }
    public string? UserName { get; set; }
    public string ActionType { get; set; } = default!;
    public DateTime ActionTimestamp { get; set; }
    public string? KeyValues { get; set; }
    public string? OldValues { get; set; }
    public string? NewValues { get; set; }
    public string? IpAddress { get; set; }
}
