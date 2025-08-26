using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NineERP.Application.Interfaces.Common;
using NineERP.Application.Interfaces.Persistence;
using NineERP.Domain.Enums;

namespace NineERP.Infrastructure.Services.Common
{
    public class IntegrationUploader(
        IApplicationDbContext context,
        IGoogleDriveUploader driveUploader,
        IS3Uploader s3Uploader,
        ILogger<IntegrationUploader> logger) : IIntegrationUploader
    {
        public async Task<string?> UploadFileAsync(IFormFile file, string? subFolder = null)
        {
            var setting = await context.IntegrationSettings
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.IsActive);

            if (setting == null)
            {
                logger.LogWarning("No active integration setting found.");
                return null;
            }

            var relativePath = subFolder ?? string.Empty;

            return setting.Type switch
            {
                IntegrationServiceType.Drive => await driveUploader.UploadFileAsync(file, relativePath, setting.ParentFolderId),
                IntegrationServiceType.S3 => await s3Uploader.UploadFileAsync(file, relativePath),
                _ => null
            };
        }
        public async Task<bool> DeleteFileAsync(string fileId)
        {
            var config = await context.IntegrationSettings
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Type == IntegrationServiceType.Drive && x.IsActive);

            if (config == null || string.IsNullOrWhiteSpace(config.ServiceAccountJson))
            {
                logger.LogWarning("Google Drive config not found or inactive.");
                return false;
            }

            var credential = GoogleCredential.FromJson(config.ServiceAccountJson)
                .CreateScoped(DriveService.Scope.Drive);

            var driveService = new DriveService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
                ApplicationName = "NineERP Google Drive Integration"
            });

            try
            {
                await driveService.Files.Delete(fileId).ExecuteAsync();
                return true;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to delete file on Google Drive. FileId: {FileId}", fileId);
                return false;
            }
        }
        public async Task<(Stream?, string?, string?)> PreviewFileAsync(string fileId)
        {
            var config = await context.IntegrationSettings
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Type == IntegrationServiceType.Drive && x.IsActive);

            if (config == null || string.IsNullOrWhiteSpace(config.ServiceAccountJson))
            {
                return (null, null, null);
            }

            var credential = GoogleCredential.FromJson(config.ServiceAccountJson)
                .CreateScoped(DriveService.Scope.Drive);

            var driveService = new DriveService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
                ApplicationName = "NineERP Google Drive Integration"
            });

            try
            {
                var file = await driveService.Files.Get(fileId).ExecuteAsync();
                var stream = new MemoryStream();
                await driveService.Files.Get(fileId).DownloadAsync(stream);
                stream.Position = 0; // ⚠️ Cực kỳ quan trọng
                return (stream, file.MimeType, file.Name);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Cannot preview file {FileId}", fileId);
                return (null, null, null);
            }
        }

    }
}
