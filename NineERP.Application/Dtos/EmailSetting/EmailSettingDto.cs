using NineERP.Domain.Enums;

namespace NineERP.Application.Dtos.EmailSetting;

public class EmailSettingDto
{
    public int Id { get; set; }
    public EmailProtocol Protocol { get; set; }
    public string? SmtpHost { get; set; }
    public int? SmtpPort { get; set; }
    public bool EnableSsl { get; set; }
    public string? SenderEmail { get; set; }
    public string? SenderName { get; set; }
    public string? SmtpUser { get; set; }
    public string? SmtpPassword { get; set; }
    public string? ClientId { get; set; }
    public string? ClientSecret { get; set; }
    public string? RefreshToken { get; set; }
    public string? AccessToken { get; set; }
    public string? TenantId { get; set; }
    public bool IsActive { get; set; }
}
