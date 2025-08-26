using NineERP.Domain.Entities.Base;
using NineERP.Domain.Enums;
namespace NineERP.Domain.Entities;
public class EmailSetting : AuditableBaseEntity<int>
{
    public EmailProtocol Protocol { get; set; } // Enum
    public string? SmtpHost { get; set; }
    public int? SmtpPort { get; set; }
    public bool EnableSsl { get; set; }

    public string? SenderEmail { get; set; }
    public string? SenderName { get; set; }
    public string? SmtpUser { get; set; }
    public string? SmtpPassword { get; set; }

    // OAuth fields
    public string? ClientId { get; set; }
    public string? ClientSecret { get; set; }
    public string? RefreshToken { get; set; }
    public string? AccessToken { get; set; }
    public string? TenantId { get; set; } // Microsoft OAuth

    public bool IsActive { get; set; } = true;
}
