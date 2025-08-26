using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using NineERP.Application.Interfaces.Common;
using NineERP.Domain.Entities;
using System.Security.Claims;
using NineERP.Application.Dtos.AuditLog;

namespace NineERP.Web.Controllers
{
    public class ErrorController(ILogger<ErrorController> logger, IAuditLogService auditLogService)
        : Controller
    {
        [Route("AccessDenied")]
        public IActionResult AccessDenied()
        {
            return View("AccessDenied");
        }

        [Route("Error/{statusCode}")]
        public async Task<IActionResult> HttpStatusCodeHandler(int statusCode)
        {
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            string? userName = User.Identity?.Name;
            string ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";

            string actionType = $"Error {statusCode}";

            logger.LogWarning("StatusCode {StatusCode} at {Path} by {User}", statusCode, HttpContext.Request.Path, userName);

            var auditEntry = new AuditLogDto
            {
                TableName = "System",
                ActionType = actionType,
                KeyValues = $"Path: {HttpContext.Request.Path}",
                UserId = userId,
                UserName = userName,
                IpAddress = ipAddress,
                ActionTimestamp = DateTime.UtcNow
            };

            await auditLogService.LogAsync(auditEntry);

            return statusCode switch
            {
                403 => View("AccessDenied"),
                404 => View("Error404"),
                _ => View("Error500")
            };
        }

        [Route("Error/Exception")]
        public async Task<IActionResult> Error()
        {
            var exceptionFeature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
            if (exceptionFeature != null)
            {
                string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                string? userName = User.Identity?.Name;
                string ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";

                logger.LogError(exceptionFeature.Error, "Unhandled exception at {Path} by {User}", exceptionFeature.Path, userName);

                var auditEntry = new AuditLogDto
                {
                    TableName = "System",
                    ActionType = "Exception",
                    KeyValues = $"Path: {exceptionFeature.Path}",
                    OldValues = exceptionFeature.Error.Message,
                    NewValues = exceptionFeature.Error.StackTrace,
                    UserId = userId,
                    UserName = userName,
                    IpAddress = ipAddress,
                    ActionTimestamp = DateTime.UtcNow
                };

                await auditLogService.LogAsync(auditEntry);
            }

            return View("Error500");
        }
    }
}
