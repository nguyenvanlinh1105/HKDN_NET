using Amazon.S3;
using Amazon.S3.Model;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NineERP.Application.Interfaces.Persistence;
using NineERP.Application.Wrapper;
using NineERP.Domain.Enums;

namespace NineERP.Application.Features.IntegrationSettingsFeature.Commands;

public class CheckS3BucketAccessCommand : IRequest<IResult>
{
    public class Handler(IApplicationDbContext context, ILogger<Handler> logger) : IRequestHandler<CheckS3BucketAccessCommand, IResult>
    {
        public async Task<IResult> Handle(CheckS3BucketAccessCommand request, CancellationToken cancellationToken)
        {
            var config = await context.IntegrationSettings.AsNoTracking()
                .FirstOrDefaultAsync(x => x.Type == IntegrationServiceType.S3 && x.IsActive, cancellationToken);

            if (config == null)
                return await Result.FailAsync("Missing S3 configuration.");

            try
            {
                var client = new AmazonS3Client(config.AccessKey, config.SecretKey, Amazon.RegionEndpoint.GetBySystemName(config.Region));
                var requestList = new ListObjectsV2Request
                {
                    BucketName = config.BucketName,
                    MaxKeys = 1
                };
                var response = await client.ListObjectsV2Async(requestList, cancellationToken);

                return await Result.SuccessAsync("S3 bucket access verified.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "S3 bucket access failed.");
                return await Result.FailAsync("S3 bucket access failed: " + ex.Message);
            }
        }
    }
}
