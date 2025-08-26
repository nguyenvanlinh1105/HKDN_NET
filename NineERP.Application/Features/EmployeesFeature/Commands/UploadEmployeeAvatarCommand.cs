using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using NineERP.Application.Dtos.Employees;
using NineERP.Application.Interfaces.Common;
using NineERP.Application.Interfaces.Persistence;
using NineERP.Application.Wrapper;
using NineERP.Domain.Entities.Dat;
using System.Text.RegularExpressions;
using System.Transactions;

namespace NineERP.Application.Features.EmployeesFeature.Commands;

public class UploadEmployeeAvatarCommand : IRequest<IResult<UploadedImageDto>>
{
    public IFormFile File { get; set; } = default!;
    public string EmployeeNo { get; set; } = default!;

    public class Handler(
        IApplicationDbContext context,
        IIntegrationUploader uploader
    ) : IRequestHandler<UploadEmployeeAvatarCommand, IResult<UploadedImageDto>>
    {
        public async Task<IResult<UploadedImageDto>> Handle(UploadEmployeeAvatarCommand request, CancellationToken cancellationToken)
        {
            // ✅ Lấy đuôi file gốc
            var extension = Path.GetExtension(request.File.FileName)?.ToLowerInvariant();
            if (string.IsNullOrWhiteSpace(extension))
                extension = ".jpg"; // fallback

            // ✅ Đặt tên chuẩn avatar
            var fileName = $"avatar{extension}";
            var uploadPath = $"employees/{request.EmployeeNo}/avatars";

            // ✅ Tìm nhân viên
            var employee = await context.DatEmployees
                .FirstOrDefaultAsync(x => x.EmployeeNo == request.EmployeeNo && !x.IsDeleted, cancellationToken);

            if (employee == null)
                return await Result<UploadedImageDto>.FailAsync("Employee not found.");

            var oldImageUrl = employee.ImageURL;

            // ✅ Upload ảnh
            var uploadUrl = await uploader.UploadFileAsync(request.File, uploadPath);
            if (string.IsNullOrWhiteSpace(uploadUrl))
                return await Result<UploadedImageDto>.FailAsync("Failed to upload avatar.");

            var fileId = ExtractDriveFileId(uploadUrl);
            if (string.IsNullOrWhiteSpace(fileId))
                return await Result<UploadedImageDto>.FailAsync("Invalid file ID.");

            employee.ImageURL = fileId;


            try
            {
                using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    await context.SaveChangesAsync(cancellationToken);

                    if (!string.IsNullOrWhiteSpace(oldImageUrl))
                    {
                        var oldFileId = ExtractDriveFileId(oldImageUrl);
                        if (!string.IsNullOrEmpty(oldFileId))
                        {
                            await uploader.DeleteFileAsync(oldFileId);
                        }
                    }

                    scope.Complete();
                }
            }
            catch (Exception ex)
            {
                return await Result<UploadedImageDto>.FailAsync("Error saving avatar. " + ex.Message);
            }

            return await Result<UploadedImageDto>.SuccessAsync(new UploadedImageDto
            {
                FileUrl = $"/preview/avatar/{fileId}",
                FileName = fileName
            });
        }

        private static string ExtractDriveFileId(string url)
        {
            var match = Regex.Match(url, @"(?:id=|/d/)([a-zA-Z0-9_-]{25,})");
            return match.Success ? match.Groups[1].Value : string.Empty;
        }
    }
}
