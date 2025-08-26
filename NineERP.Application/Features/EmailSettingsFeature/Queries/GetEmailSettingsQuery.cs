using MediatR;
using Microsoft.EntityFrameworkCore;
using NineERP.Application.Dtos.EmailSetting;
using NineERP.Application.Interfaces.Persistence;
using NineERP.Application.Wrapper;

namespace NineERP.Application.Features.EmailSettingsFeature.Queries
{
    public record GetEmailSettingsQuery() : IRequest<Result<EmailSettingDto>>
    {
        public class Handler(IApplicationDbContext context) : IRequestHandler<GetEmailSettingsQuery, Result<EmailSettingDto>>
        {
            public async Task<Result<EmailSettingDto>> Handle(GetEmailSettingsQuery request, CancellationToken cancellationToken)
            {
                var setting = await context.EmailSettings
                    .OrderBy(x => x.Id)
                    .FirstOrDefaultAsync(cancellationToken);

                if (setting is null)
                {
                    return await Result<EmailSettingDto>.FailAsync("No Email Settings found.");
                }

                var dto = new EmailSettingDto
                {
                    Protocol = setting.Protocol,
                    SmtpHost = setting.SmtpHost,
                    SmtpPort = setting.SmtpPort,
                    EnableSsl = setting.EnableSsl,
                    SenderEmail = setting.SenderEmail,
                    SenderName = setting.SenderName,
                    SmtpUser = setting.SmtpUser,
                    SmtpPassword = setting.SmtpPassword,
                    ClientId = setting.ClientId,
                    ClientSecret = setting.ClientSecret,
                    RefreshToken = setting.RefreshToken,
                    AccessToken = setting.AccessToken,
                    TenantId = setting.TenantId
                };

                return await Result<EmailSettingDto>.SuccessAsync(dto);
            }
        }
    }
}
