using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NineERP.Application.Dtos.EmailSetting;
using NineERP.Application.Features.EmailSettingsFeature.Commands;
using NineERP.Application.Features.EmailSettingsFeature.Queries;
using NineERP.Application.Interfaces.Common;
using NineERP.Domain.Enums;
using NineERP.Infrastructure.Helpers;
using System.Security.Claims;
using MediatR;
using NineERP.Application.Constants.Role;
using Microsoft.AspNetCore.Authentication.Cookies;
using NineERP.Application.Dtos.AuditLog;

namespace NineERP.Web.Controllers;

[Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
public class EmailSettingsController(
    IMediator mediator,
    IAuditLogService auditLogService,
    ILogger<EmailSettingsController> logger,
    ICurrentUserService currentUser
) : Controller
{
    [HttpGet]
    [Authorize(Policy = $"Permission:{PermissionValue.EmailSettings.View}")]
    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    [Route("EmailSettings/GetData")]
    [Authorize(Policy = $"Permission:{PermissionValue.EmailSettings.View}")]
    public async Task<IActionResult> GetData()
    {
        var result = await mediator.Send(new GetEmailSettingsQuery());
        return Ok(result.Data); // Return data for binding to form
    }

    [HttpPost]
    [Authorize(Policy = $"Permission:{PermissionValue.EmailSettings.Update}")]
    public async Task<IActionResult> SaveEmailSettings([FromForm] EmailSettingRequest request)
    {
        try
        {
            var result = await mediator.Send(new UpdateEmailSettingsCommand(request));

            // Log audit entry on success
            if (result.Succeeded)
            {
                var auditEntry = new AuditLogDto
                {
                    TableName = "EmailSettings",
                    ActionType = "Update",
                    NewValues = $"Protocol: {request.Protocol}, SenderEmail: {request.SenderEmail}, SenderName: {request.SenderName}",
                    ActionTimestamp = DateTime.UtcNow,
                    UserId = currentUser.UserId ?? "SYSTEM",
                    UserName = currentUser.UserName ?? "SYSTEM",
                    IpAddress = currentUser.Origin
                };

                await auditLogService.LogAsync(auditEntry); // Log async to avoid blocking

            }

            return Json(result);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error while saving email settings");
            return Json(new { success = false, message = "An error occurred while saving the email configuration." });
        }
    }

    [HttpPost]
    [Authorize(Policy = $"Permission:{PermissionValue.EmailSettings.View}")]
    public async Task<IActionResult> SendTestEmail([FromForm] string testEmail)
    {
        try
        {
            var result = await mediator.Send(new SendTestEmailCommand(testEmail));

            // Log audit entry for sending test email
            var auditEntry = new AuditLogDto
            {
                TableName = "EmailSettings",
                ActionType = "SendTestEmail",
                NewValues = $"TestEmail: {testEmail}",
                ActionTimestamp = DateTime.UtcNow,
                UserId = currentUser.UserId ?? RoleConstants.SuperAdmin,
                UserName = currentUser.UserName ?? RoleConstants.SuperAdmin,
                IpAddress = currentUser.Origin
            };

            await auditLogService.LogAsync(auditEntry); // Log async to avoid blocking

            return Json(result);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error while sending test email to {Email}", testEmail);
            return Json(new { success = false, message = "Failed to send test email." });
        }
    }
}
