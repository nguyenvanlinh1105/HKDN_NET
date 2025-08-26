using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Drive.v3.Data;
using Google.Apis.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using NineERP.Application.Interfaces.Common;
using NineERP.Application.Interfaces.Persistence;
using NineERP.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using File = Google.Apis.Drive.v3.Data.File;

namespace NineERP.Infrastructure.Services.Common
{
    public class GoogleDriveUploader(
        ILogger<GoogleDriveUploader> logger,
        IApplicationDbContext context
    ) : IGoogleDriveUploader
    {
        public async Task<string?> UploadFileAsync(IFormFile file, string relativePath, string? parentFolderId = null)
        {
            var config = await context.IntegrationSettings
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Type == IntegrationServiceType.Drive && x.IsActive);

            if (config == null || string.IsNullOrWhiteSpace(config.ServiceAccountJson))
            {
                logger.LogWarning("No active Google Drive integration found.");
                return null;
            }

            var credential = GoogleCredential.FromJson(config.ServiceAccountJson)
                .CreateScoped(DriveService.Scope.Drive);

            var driveService = new DriveService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
                ApplicationName = "NineERP Google Drive Integration"
            });

            var folders = relativePath.Split('/', StringSplitOptions.RemoveEmptyEntries);
            string parentId = parentFolderId ?? config.ParentFolderId ?? "root";

            foreach (var folderName in folders)
            {
                parentId = await FindOrCreateFolder(driveService, folderName, parentId);
            }

            var fileMetadata = new File
            {
                Name = file.FileName,
                Parents = new List<string> { parentId }
            };

            using var stream = file.OpenReadStream();

            var request = driveService.Files.Create(fileMetadata, stream, file.ContentType);
            request.Fields = "id";
            var uploadResult = await request.UploadAsync();

            if (uploadResult.Status != Google.Apis.Upload.UploadStatus.Completed)
            {
                logger.LogError("Google Drive upload failed: {Error}", uploadResult.Exception?.Message);
                return null;
            }

            var fileId = request.ResponseBody.Id;

            var permission = new Permission { Type = "anyone", Role = "reader" };
            await driveService.Permissions.Create(permission, fileId).ExecuteAsync();

            return $"https://drive.google.com/uc?id={fileId}";
        }
        private async Task<string> FindOrCreateFolder(DriveService service, string name, string parentId)
        {
            var listRequest = service.Files.List();
            listRequest.Q = $"mimeType = 'application/vnd.google-apps.folder' and name = '{name}' and trashed = false and '{parentId}' in parents";
            listRequest.Fields = "files(id, name)";
            var result = await listRequest.ExecuteAsync();

            var folder = result.Files.FirstOrDefault();
            if (folder != null)
            {
                return folder.Id;
            }

            var metadata = new File
            {
                Name = name,
                MimeType = "application/vnd.google-apps.folder",
                Parents = new List<string> { parentId }
            };

            var created = await service.Files.Create(metadata).ExecuteAsync();
            return created.Id;
        }
    }
}
