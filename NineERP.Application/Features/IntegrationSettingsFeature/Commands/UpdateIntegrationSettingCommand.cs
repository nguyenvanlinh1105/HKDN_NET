using MediatR;
using Microsoft.EntityFrameworkCore;
using NineERP.Application.Dtos.IntegrationSetting;
using NineERP.Application.Interfaces.Persistence;
using NineERP.Application.Wrapper;
using NineERP.Domain.Entities;
using NineERP.Domain.Enums;

namespace NineERP.Application.Features.IntegrationSettingsFeature.Commands;
public class UpdateIntegrationSettingCommand(IntegrationSettingRequest model) : IRequest<IResult>
{
    public IntegrationSettingRequest Model { get; set; } = model;

    public class Handler(IApplicationDbContext context) : IRequestHandler<UpdateIntegrationSettingCommand, IResult>
    {
        public async Task<IResult> Handle(UpdateIntegrationSettingCommand request, CancellationToken cancellationToken)
        {
            var type = Enum.Parse<IntegrationServiceType>(request.Model.Type);

            // Lấy dòng đang cập nhật
            var setting = await context.IntegrationSettings
                .FirstOrDefaultAsync(x => x.Type == type, cancellationToken);

            if (setting == null)
            {
                setting = new IntegrationSetting { Type = type };
                await context.IntegrationSettings.AddAsync(setting, cancellationToken);
            }

            // Nếu bật active thì tắt những dòng Drive/S3 khác
            if (request.Model.IsActive && (type == IntegrationServiceType.Drive || type == IntegrationServiceType.S3))
            {
                var otherTypes = new[] { IntegrationServiceType.Drive, IntegrationServiceType.S3 }
                    .Where(t => t != type);

                var others = await context.IntegrationSettings
                    .Where(x => otherTypes.Contains(x.Type))
                    .ToListAsync(cancellationToken);

                foreach (var item in others)
                {
                    item.IsActive = false;
                }
            }

            // Gán dữ liệu từ request
            setting.ClientId = request.Model.ClientId;
            setting.ClientSecret = request.Model.ClientSecret;
            setting.ServiceAccountJson = request.Model.ServiceAccountJson;
            setting.PublicKey = request.Model.PublicKey;
            setting.PrivateKey = request.Model.PrivateKey;
            setting.AccessKey = request.Model.AccessKey;
            setting.SecretKey = request.Model.SecretKey;
            setting.Region = request.Model.Region;
            setting.BucketName = request.Model.BucketName;
            setting.ParentFolderId = request.Model.ParentFolderId;
            setting.IsActive = request.Model.IsActive;

            var result = await context.SaveChangesAsync(cancellationToken);
            return result > 0
                ? await Result.SuccessAsync()
                : await Result.FailAsync("Failed to update Integration Setting.");
        }
    }
}

