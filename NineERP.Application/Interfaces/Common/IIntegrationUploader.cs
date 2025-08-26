using Microsoft.AspNetCore.Http;
using NineERP.Domain.Enums;
using System.Threading.Tasks;

namespace NineERP.Application.Interfaces.Common
{
    public interface IIntegrationUploader
    {
        Task<string?> UploadFileAsync(IFormFile file, string? subFolder = null);
        Task<bool> DeleteFileAsync(string fileId);
        Task<(Stream?, string?, string?)> PreviewFileAsync(string fileId);
    }
}
