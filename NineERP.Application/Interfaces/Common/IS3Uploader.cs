using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace NineERP.Application.Interfaces.Common
{
    public interface IS3Uploader
    {
        Task<string?> UploadFileAsync(IFormFile file, string relativePath);
    }
}
