using Microsoft.AspNetCore.Mvc;
using NineERP.Application.Interfaces.Common;

namespace NineERP.Web.Controllers;

[Route("preview")]
public class PreviewController : Controller
{
    private readonly IIntegrationUploader _uploader;

    public PreviewController(IIntegrationUploader uploader)
    {
        _uploader = uploader;
    }

    [HttpGet("file")]
    public async Task<IActionResult> FileById([FromQuery] string id)
    {
        var (stream, mimeType, fileName) = await _uploader.PreviewFileAsync(id);
        if (stream == null)
        {
            Console.WriteLine("❌ Stream is null for file id: " + id);
            return NotFound();
        }

        var ext = Path.GetExtension(fileName ?? "").ToLowerInvariant();
        var contentType = ext == ".pdf" ? "application/pdf" : mimeType ?? "application/octet-stream";

        // 🔒 Fix lỗi non-ASCII bằng cách đặt tên an toàn
        var safeFileName = "preview" + ext;

        return File(stream, contentType, safeFileName);
    }
    [HttpGet("avatar/{id}")]
    public async Task<IActionResult> AvatarById(string id)
    {
        var (stream, mimeType, fileName) = await _uploader.PreviewFileAsync(id);
        if (stream == null)
        {
            Console.WriteLine("❌ Stream is null for avatar id: " + id);
            return NotFound();
        }

        var contentType = mimeType ?? "image/jpeg";

        // ✅ Không cần tên file khi hiển thị ảnh avatar
        return File(stream, contentType);
    }

}
