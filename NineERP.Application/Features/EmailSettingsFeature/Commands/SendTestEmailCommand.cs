using MediatR;
using Microsoft.EntityFrameworkCore;
using NineERP.Application.Interfaces.Common;
using NineERP.Application.Interfaces.Persistence;
using NineERP.Application.Wrapper;

namespace NineERP.Application.Features.EmailSettingsFeature.Commands
{
    public class SendTestEmailCommand(string toEmail) : IRequest<IResult>
    {
        public string ToEmail { get; set; } = toEmail;

        public class Handler(
            IApplicationDbContext context,
            IEmailService emailService
        ) : IRequestHandler<SendTestEmailCommand, IResult>
        {
            public async Task<IResult> Handle(SendTestEmailCommand request, CancellationToken cancellationToken)
            {
                var config = await context.EmailSettings
                    .OrderBy(x => x.Id)
                    .FirstOrDefaultAsync(cancellationToken);

                if (config == null)
                    return await Result.FailAsync("Email settings are not configured.");

                var subject = "Test Email";
                var body = "This is a test email from the system to verify SMTP settings.";

                try
                {
                    await emailService.SendAsync(request.ToEmail, subject, body);
                    return await Result.SuccessAsync("Test email sent successfully.");
                }
                catch (Exception ex)
                {
                    return await Result.FailAsync($"Failed to send test email: {ex.Message}");
                }
            }
        }
    }
}
