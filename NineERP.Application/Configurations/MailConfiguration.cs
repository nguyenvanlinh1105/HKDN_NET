namespace NineERP.Application.Configurations
{
    public class MailConfiguration
    {
        public string From { get; set; } = default!;
        public string Host { get; set; } = default!;
        public int Port { get; set; }
        public string UserName { get; set; } = default!;
        public string Password { get; set; } = default!;
        public string DisplayName { get; set; } = default!;
        public bool? SmtpUseSsl { get; set; }
        public string? TestEmail { get; set; } = default!;
    }
}