using ERP.Domain.Entities.DatTable.Employee;
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

public class UploadEmployeeIdentityCommand : IRequest<IResult<UploadedImageDto>>
{
    public IFormFile File { get; set; } = default!;
    public string EmployeeNo { get; set; } = default!;
    public string Side { get; set; } = default!; // "front" or "back"

    public class Handler(
        IApplicationDbContext context,
        IIntegrationUploader uploader
    ) : IRequestHandler<UploadEmployeeIdentityCommand, IResult<UploadedImageDto>>
    {
        public async Task<IResult<UploadedImageDto>> Handle(UploadEmployeeIdentityCommand request, CancellationToken cancellationToken)
        {
            // ✅ Lấy đuôi file từ tên gốc
            var extension = Path.GetExtension(request.File.FileName)?.ToLowerInvariant();
            if (string.IsNullOrWhiteSpace(extension))
                extension = ".jpg"; // fallback nếu không có

            // ✅ Tên file: NPLUS0001_front.jpg hoặc NPLUS0001_back.png
            var fileName = request.Side.ToLower() switch
            {
                "front" => $"{request.EmployeeNo}_front{extension}",
                "back" => $"{request.EmployeeNo}_back{extension}",
                _ => $"{request.EmployeeNo}_other{extension}"
            };

            // ✅ Đường dẫn thư mục (không theo mã nhân viên)
            var uploadPath = $"employees/{request.EmployeeNo}/identity";

            // ✅ Kiểm tra nhân viên
            var employee = await context.DatEmployees
                .FirstOrDefaultAsync(x => x.EmployeeNo == request.EmployeeNo && !x.IsDeleted, cancellationToken);
            if (employee == null)
                return await Result<UploadedImageDto>.FailAsync("Employee not found.");

            // ✅ Lấy hoặc tạo mới thông tin CCCD
            var identity = await context.DatEmployeeIdentities
                .FirstOrDefaultAsync(x => x.EmployeeNo == request.EmployeeNo, cancellationToken);

            if (identity == null)
            {
                identity = new DatEmployeeIdentity
                {
                    EmployeeNo = request.EmployeeNo
                };
                context.DatEmployeeIdentities.Add(identity);
            }

            // ✅ Ghi nhớ ảnh cũ
            string? oldImageUrl = request.Side.ToLower() switch
            {
                "front" => identity.PhotoBeforeCitizenshipCard,
                "back" => identity.PhotoAfterCitizenshipCard,
                _ => null
            };

            // ✅ Upload ảnh
            var uploadUrl = await uploader.UploadFileAsync(request.File, uploadPath);
            if (string.IsNullOrWhiteSpace(uploadUrl))
                return await Result<UploadedImageDto>.FailAsync("Failed to upload identity image.");

            // ✅ Trích xuất thumbnail URL từ Google Drive
            var fileId = ExtractDriveFileId(uploadUrl);
            var thumbnailUrl = $"https://drive.google.com/thumbnail?id={fileId}";

            // ✅ Lưu ảnh mới vào DB
            if (request.Side.ToLower() == "front")
                identity.PhotoBeforeCitizenshipCard = thumbnailUrl;
            else if (request.Side.ToLower() == "back")
                identity.PhotoAfterCitizenshipCard = thumbnailUrl;

            // ✅ Save & xóa ảnh cũ nếu có
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
                return await Result<UploadedImageDto>.FailAsync("Error saving identity image. " + ex.Message);
            }

            return await Result<UploadedImageDto>.SuccessAsync(new UploadedImageDto
            {
                FileUrl = thumbnailUrl,
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
