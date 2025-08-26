using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MimeKit;
using NineERP.Application.Interfaces.Common;
using NineERP.Application.Interfaces.Persistence;
using NineERP.Domain.Enums;
using System;
using System.Threading.Tasks;

namespace NineERP.Infrastructure.Services.Common
{
    public class EmailService(
        IApplicationDbContext context,
        ILogger<EmailService> logger
    ) : IEmailService
    {
        public async Task SendAsync(string to, string subject, string body)
        {
            try
            {
                var config = await context.EmailSettings
                    .Where(x => x.IsActive)
                    .OrderBy(x => x.Id)
                    .FirstOrDefaultAsync();

                if (config == null)
                {
                    logger.LogWarning("No active email settings found.");
                    return;
                }

                var message = new MimeMessage
                {
                    Sender = new MailboxAddress(config.SenderName ?? config.SenderEmail, config.SenderEmail)
                };
                message.From.Add(message.Sender);
                message.To.Add(MailboxAddress.Parse(to));
                message.Subject = subject;
                message.Body = new BodyBuilder { HtmlBody = body }.ToMessageBody();

                switch (config.Protocol)
                {
                    case EmailProtocol.SMTP:
                        await SendUsingSmtp(config, message);
                        break;

                    case EmailProtocol.GmailOAuth:
                        await SendUsingGmailOAuth(config, message);
                        break;

                    case EmailProtocol.MicrosoftOAuth:
                        await SendUsingMicrosoftOAuth(config, message);
                        break;

                    default:
                        logger.LogWarning("Unknown email protocol: {Protocol}", config.Protocol);
                        break;
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to send email to {To}: {Message}", to, ex.Message);
            }
        }

        private async Task SendUsingSmtp(Domain.Entities.EmailSetting config, MimeMessage message)
        {
            using var smtp = new SmtpClient();

            var sslOption = config.EnableSsl ? SecureSocketOptions.StartTls : SecureSocketOptions.Auto;
            await smtp.ConnectAsync(config.SmtpHost, config.SmtpPort ?? 587, sslOption);

            if (!string.IsNullOrWhiteSpace(config.SmtpUser) && !string.IsNullOrWhiteSpace(config.SmtpPassword))
            {
                await smtp.AuthenticateAsync(config.SmtpUser, config.SmtpPassword);
            }

            await smtp.SendAsync(message);
            await smtp.DisconnectAsync(true);
        }

        private async Task SendUsingGmailOAuth(Domain.Entities.EmailSetting config, MimeMessage message)
        {
            if (string.IsNullOrWhiteSpace(config.AccessToken) || string.IsNullOrWhiteSpace(config.SenderEmail))
            {
                logger.LogError("Gmail OAuth configuration is missing AccessToken or SenderEmail.");
                return;
            }

            using var smtp = new SmtpClient();

            await smtp.ConnectAsync("smtp.gmail.com", 587, SecureSocketOptions.StartTls);

            var oauth2 = new SaslMechanismOAuth2(config.SenderEmail, config.AccessToken);
            await smtp.AuthenticateAsync(oauth2);

            await smtp.SendAsync(message);
            await smtp.DisconnectAsync(true);
        }

        private async Task SendUsingMicrosoftOAuth(Domain.Entities.EmailSetting config, MimeMessage message)
        {
            if (string.IsNullOrWhiteSpace(config.AccessToken) || string.IsNullOrWhiteSpace(config.SenderEmail))
            {
                logger.LogError("Microsoft OAuth configuration is missing AccessToken or SenderEmail.");
                return;
            }

            using var smtp = new SmtpClient();
            await smtp.ConnectAsync("smtp.office365.com", 587, SecureSocketOptions.StartTls);

            var oauth2 = new SaslMechanismOAuth2(config.SenderEmail, config.AccessToken);
            await smtp.AuthenticateAsync(oauth2);

            await smtp.SendAsync(message);
            await smtp.DisconnectAsync(true);
        }
    }
}
