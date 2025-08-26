using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using NineERP.Application.Interfaces.Common;
using NineERP.Application.Wrapper;

namespace NineERP.Application.Features.IntegrationSettingsFeature.Commands
{
    public class TestUploadIntegrationCommand(IFormFile file, string? subFolder) : IRequest<IResult<string>>
    {
        public IFormFile File { get; set; } = file;
        public string? SubFolder { get; set; } = subFolder;

        public class Handler(
            IIntegrationUploader uploader,
            ILogger<Handler> logger) : IRequestHandler<TestUploadIntegrationCommand, IResult<string>>
        {
            public async Task<IResult<string>> Handle(TestUploadIntegrationCommand request, CancellationToken cancellationToken)
            {
                try
                {
                    var uploadedUrl = await uploader.UploadFileAsync(request.File, request.SubFolder);
                    if (string.IsNullOrWhiteSpace(uploadedUrl))
                        return await Result<string>.FailAsync("Upload failed. No URL returned.");

                    logger.LogInformation("Test file uploaded successfully to: {Url}", uploadedUrl);
                    return await Result<string>.SuccessAsync(uploadedUrl);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Test upload failed.");
                    return await Result<string>.FailAsync("Error during test upload: " + ex.Message);
                }
            }
        }
    }
}
