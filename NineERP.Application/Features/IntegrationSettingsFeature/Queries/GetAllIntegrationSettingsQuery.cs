using MediatR;
using Microsoft.EntityFrameworkCore;
using NineERP.Application.Dtos.IntegrationSetting;
using NineERP.Application.Interfaces.Persistence;
using NineERP.Application.Wrapper;

namespace NineERP.Application.Features.IntegrationSettingsFeature.Queries
{
    public record GetAllIntegrationSettingsQuery : IRequest<Result<List<IntegrationSettingDto>>>
    {
        public class Handler(IApplicationDbContext context)
            : IRequestHandler<GetAllIntegrationSettingsQuery, Result<List<IntegrationSettingDto>>>
        {
            public async Task<Result<List<IntegrationSettingDto>>> Handle(GetAllIntegrationSettingsQuery request, CancellationToken cancellationToken)
            {
                var settings = await context.IntegrationSettings.AsNoTracking().ToListAsync(cancellationToken);

                var result = settings.Select(x => new IntegrationSettingDto
                {
                    Type = x.Type.ToString(),
                    ClientId = x.ClientId,
                    ClientSecret = x.ClientSecret,
                    PublicKey = x.PublicKey,
                    PrivateKey = x.PrivateKey,
                    ServiceAccountJson = x.ServiceAccountJson,
                    AccessKey = x.AccessKey,
                    SecretKey = x.SecretKey,
                    Region = x.Region,
                    BucketName = x.BucketName,
                    ParentFolderId = x.ParentFolderId,
                    IsActive = x.IsActive
                }).ToList();

                return await Result<List<IntegrationSettingDto>>.SuccessAsync(result);
            }
        }
    }
}
