using Amazon.S3;
using Amazon.S3.Transfer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using NineERP.Application.Interfaces.Common;
using NineERP.Application.Interfaces.Persistence;
using NineERP.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace NineERP.Infrastructure.Services.Common
{
    public class S3Uploader(
        ILogger<S3Uploader> logger,
        IApplicationDbContext context
    ) : IS3Uploader
    {
        public async Task<string?> UploadFileAsync(IFormFile file, string relativePath)
        {
            var config = await context.IntegrationSettings
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Type == IntegrationServiceType.S3 && x.IsActive);

            if (config == null)
            {
                logger.LogWarning("No active S3 integration config found.");
                return null;
            }

            try
            {
                using var client = new AmazonS3Client(config.AccessKey, config.SecretKey, Amazon.RegionEndpoint.GetBySystemName(config.Region));
                using var stream = file.OpenReadStream();

                var key = relativePath.TrimEnd('/') + "/" + file.FileName;

                var request = new TransferUtilityUploadRequest
                {
                    InputStream = stream,
                    BucketName = config.BucketName,
                    Key = key,
                    CannedACL = S3CannedACL.PublicRead
                };

                var transferUtility = new TransferUtility(client);
                await transferUtility.UploadAsync(request);

                return $"https://{config.BucketName}.s3.{config.Region}.amazonaws.com/{key}";
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to upload file to S3.");
                return null;
            }
        }
    }
}
