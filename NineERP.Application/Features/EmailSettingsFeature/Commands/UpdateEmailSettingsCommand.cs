using MediatR;
using Microsoft.EntityFrameworkCore;
using NineERP.Application.Dtos.EmailSetting;
using NineERP.Application.Dtos.GeneralSetting;
using NineERP.Application.Interfaces.Persistence;
using NineERP.Application.Wrapper;
using NineERP.Domain.Entities;

namespace NineERP.Application.Features.EmailSettingsFeature.Commands
{
    public class UpdateEmailSettingsCommand(EmailSettingRequest model) : IRequest<IResult>
    {
        public EmailSettingRequest Model { get; set; } = model;

        public class Handler(IApplicationDbContext context) : IRequestHandler<UpdateEmailSettingsCommand, IResult>
        {
            public async Task<IResult> Handle(UpdateEmailSettingsCommand request, CancellationToken cancellationToken)
            {
                var setting = await context.EmailSettings
                    .OrderBy(x => x.Id)
                    .FirstOrDefaultAsync(cancellationToken);

                if (setting == null)
                {
                    setting = new EmailSetting();
                    await context.EmailSettings.AddAsync(setting, cancellationToken);
                }

                // Gán giá trị từ request
                setting.Protocol = request.Model.Protocol;
                setting.SmtpHost = request.Model.SmtpHost;
                setting.SmtpPort = request.Model.SmtpPort;
                setting.EnableSsl = request.Model.EnableSsl;
                setting.SenderEmail = request.Model.SenderEmail;
                setting.SenderName = request.Model.SenderName;
                setting.SmtpUser = request.Model.SmtpUser;
                setting.SmtpPassword = request.Model.SmtpPassword;
                setting.ClientId = request.Model.ClientId;
                setting.ClientSecret = request.Model.ClientSecret;
                setting.RefreshToken = request.Model.RefreshToken;
                setting.AccessToken = request.Model.AccessToken;
                setting.TenantId = request.Model.TenantId;

                var result = await context.SaveChangesAsync(cancellationToken);
                return result > 0
                    ? await Result.SuccessAsync("Email settings updated successfully.")
                    : await Result.FailAsync("Failed to update email settings.");
            }
        }
    }
}
