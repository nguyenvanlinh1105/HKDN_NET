using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace NineERP.Application.Interfaces.Common
{
    public interface IGoogleDriveUploader
    {
        Task<string?> UploadFileAsync(IFormFile file, string relativePath, string? parentFolderId = null);
    }
}
