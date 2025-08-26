using MediatR;
using Microsoft.EntityFrameworkCore;
using NineERP.Application.Interfaces.Common;
using NineERP.Application.Interfaces.Persistence;
using NineERP.Application.Wrapper;
using NineERP.Domain.Entities.Dat;

namespace NineERP.Application.Features.EmployeesFeature.Commands;

public class DeleteEmployeeDocumentCommand(long id) : IRequest<IResult>
{
    public long Id { get; set; } = id;

    public class Handler(
        IApplicationDbContext context,
        IIntegrationUploader uploader,      // ✅ Thêm uploader
        ICurrentUserService currentUser
    ) : IRequestHandler<DeleteEmployeeDocumentCommand, IResult>
    {
        public async Task<IResult> Handle(DeleteEmployeeDocumentCommand request, CancellationToken cancellationToken)
        {
            var entity = await context.DatEmployeeDocuments
                .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, cancellationToken);

            if (entity == null)
                return await Result.FailAsync("Document not found.");

            var user = currentUser.UserName ?? "System";

            // 🔹 Ghi log trước khi xóa
            var log = new DatEmployeeDocumentLog
            {
                DocumentId = entity.Id,
                EmployeeNo = entity.EmployeeNo,
                FileName = entity.FileName,
                NameFile = entity.NameFile,
                TypeFile = entity.TypeFile,
                SizeFile = entity.SizeFile,
                GoogleDriveFileId = entity.GoogleDriveFileId,
                GoogleDriveFileUrl = entity.GoogleDriveFileUrl,
                DeletedBy = user,
                DeletedOn = DateTime.UtcNow
            };
            await context.DatEmployeeDocumentLogs.AddAsync(log, cancellationToken);

            // 🔹 Xóa file thực trên Drive nếu có ID
            if (!string.IsNullOrWhiteSpace(entity.GoogleDriveFileId))
            {
                await uploader.DeleteFileAsync(entity.GoogleDriveFileId);
            }

            // 🔹 Xóa mềm
            entity.IsDeleted = true;
            entity.LastModifiedBy = user;
            entity.LastModifiedOn = DateTime.UtcNow;

            await context.SaveChangesAsync(cancellationToken);
            return await Result.SuccessAsync("Document deleted successfully.");
        }
    }
}
