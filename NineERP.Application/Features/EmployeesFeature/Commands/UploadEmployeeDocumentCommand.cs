using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using NineERP.Application.Dtos.Employees;
using NineERP.Application.Interfaces.Common;
using NineERP.Application.Interfaces.Persistence;
using NineERP.Application.Wrapper;
using NineERP.Domain.Entities.Dat;
using System.IO;
using System.Text.RegularExpressions;

namespace NineERP.Application.Features.EmployeesFeature.Commands;

public class UploadEmployeeDocumentCommand : IRequest<IResult<EmployeeDocumentDto>>
{
    public IFormFile File { get; set; } = default!;
    public string EmployeeNo { get; set; } = default!;

    public class Handler(
        IApplicationDbContext context,
        IIntegrationUploader uploader,
        ICurrentUserService currentUser) : IRequestHandler<UploadEmployeeDocumentCommand, IResult<EmployeeDocumentDto>>
    {
        public async Task<IResult<EmployeeDocumentDto>> Handle(UploadEmployeeDocumentCommand request, CancellationToken cancellationToken)
        {
            // 🟣 1. Upload to Google Drive (or S3 via IIntegrationUploader)
            var uploadUrl = await uploader.UploadFileAsync(request.File, $"employees/{request.EmployeeNo}");
            if (string.IsNullOrWhiteSpace(uploadUrl))
                return await Result<EmployeeDocumentDto>.FailAsync("Failed to upload file to storage.");

            // 🟣 2. Save metadata to DB
            var entity = new DatEmployeeDocument
            {
                EmployeeNo = request.EmployeeNo,
                FileName = request.File.FileName,
                NameFile = Path.GetFileNameWithoutExtension(request.File.FileName),
                SizeFile = GetReadableFileSize(request.File.Length),
                TypeFile = Path.GetExtension(request.File.FileName)?.TrimStart('.'),
                GoogleDriveFileUrl = uploadUrl,
                GoogleDriveFileId = ExtractDriveFileId(uploadUrl),

                CreatedBy = currentUser.UserName,
                CreatedOn = DateTime.UtcNow
            };

            context.DatEmployeeDocuments.Add(entity);
            await context.SaveChangesAsync(cancellationToken);

            // 🟣 3. Return result
            var dto = new EmployeeDocumentDto
            {
                Id = entity.Id,
                FileName = entity.FileName,
                NameFile = entity.NameFile,
                SizeFile = entity.SizeFile,
                TypeFile = entity.TypeFile,
                GoogleDriveFileUrl = entity.GoogleDriveFileUrl,
                GoogleDriveFileId = entity.GoogleDriveFileId,
                CreatedBy = entity.CreatedBy,
                CreatedOn = entity.CreatedOn,
                LastModifiedBy = entity.LastModifiedBy,
                LastModifiedOn = entity.LastModifiedOn
            };

            return await Result<EmployeeDocumentDto>.SuccessAsync(dto);
        }

        private static string GetReadableFileSize(long bytes)
        {
            var size = bytes / 1024.0;
            if (size < 1024)
                return size.ToString("N0") + " KB";
            else
                return (size / 1024.0).ToString("N2") + " MB";
        }
        private static string ExtractDriveFileId(string url)
        {
            var match = Regex.Match(url, @"(?:id=|/d/)([a-zA-Z0-9_-]+)");
            return match.Success ? match.Groups[1].Value : url;
        }

    }
}
