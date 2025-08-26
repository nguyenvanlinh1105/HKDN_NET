using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NineERP.Application.Interfaces.Persistence;
using NineERP.Application.Wrapper;
using NineERP.Domain.Enums;
using System.Text.Json;

namespace NineERP.Application.Features.IntegrationSettingsFeature.Commands;

public class TestIntegrationSettingCommand : IRequest<IResult>
{
    public class Handler(IApplicationDbContext context, ILogger<Handler> logger) : IRequestHandler<TestIntegrationSettingCommand, IResult>
    {
        public async Task<IResult> Handle(TestIntegrationSettingCommand request, CancellationToken cancellationToken)
        {
            var config = await context.IntegrationSettings.AsNoTracking()
                .FirstOrDefaultAsync(x => x.Type == IntegrationServiceType.Drive && x.IsActive, cancellationToken);

            if (config == null || string.IsNullOrWhiteSpace(config.ServiceAccountJson))
                return await Result.FailAsync("Missing Google Drive credentials");

            try
            {
                var credential = GoogleCredential.FromJson(config.ServiceAccountJson)
                    .CreateScoped(DriveService.Scope.Drive);

                var service = new DriveService(new BaseClientService.Initializer
                {
                    HttpClientInitializer = credential,
                    ApplicationName = "NineERP"
                });

                // Lấy quyền xem thư mục gốc
                var requestFile = service.Files.Get("root");
                requestFile.Fields = "id, name";
                var file = await requestFile.ExecuteAsync(cancellationToken);

                logger.LogInformation("Google Drive root access: {Name} ({Id})", file.Name, file.Id);
                return await Result.SuccessAsync("Google Drive integration works.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Google Drive integration failed");
                return await Result.FailAsync("Google Drive test failed: " + ex.Message);
            }
        }
    }
}
