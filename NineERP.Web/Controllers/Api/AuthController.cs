using Hangfire;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NineERP.Application.Constants.Messages;
using NineERP.Application.Features.AuthFeature.Commands;
using NineERP.Application.Wrapper;
using NineERP.Infrastructure.Helpers;
using NineERP.Web.Models;
using Serilog;
using System.Globalization;
using Microsoft.EntityFrameworkCore;
using NineERP.Application.Interfaces.Common;
using NineERP.Domain.Entities.Identity;
using NineERP.Infrastructure.Contexts;
using NineERP.Application.Dtos.AuditLog;

namespace NineERP.Web.Controllers.Api
{
    [ApiController]
    [Route("api/auth")]
    [ApiExplorerSettings(GroupName = "v1")]
    public class AuthController(
        IMediator mediator, 
        UserManager<AppUser> userManager,
        IEmailService emailService, 
        ApplicationDbContext context, 
        IAuditLogService auditLogService) : ControllerBase
    {
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginCommand command)
        {
            var response = await mediator.Send(command);
            return new ObjectResult(response) { StatusCode = response.Status };
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenCommand command)
        {
            var response = await mediator.Send(command);
            return new ObjectResult(response) { StatusCode = response.Status };
        }

        [HttpPost("logout")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> Logout()
        {
            var username = User.Identity?.Name;
            if (string.IsNullOrEmpty(username))
                return new ObjectResult(GenericResponse<object>.ErrorResponse(401, ErrorMessages.GetMessage("AUTH005"), "AUTH005", ErrorMessages.GetMessage("AUTH005"))) { StatusCode = 401 };

            var response = await mediator.Send(new LogoutCommand { UserName = username });
            return new ObjectResult(response) { StatusCode = response.Status };
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            try
            {
                var user = await userManager.FindByEmailAsync(model.Email);
                if (user is null || !await userManager.IsEmailConfirmedAsync(user))
                {
                    Log.Warning("ForgotPassword - user not found or email unconfirmed: {Email}", model.Email);
                    return new ObjectResult(GenericResponse<object>.ErrorResponse(404, ErrorMessages.GetMessage("DATA001"), "DATA001", ErrorMessages.GetMessage("DATA001"))) { StatusCode = 404 };
                }

                var token = await userManager.GeneratePasswordResetTokenAsync(user);
                var resetLink = Url.Action("ResetPassword", "ForgotPassword", new { token, email = model.Email }, Request.Scheme);

                var culture = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;

                var template = await context.EmailTemplates.AsNoTracking()
                    .Where(x => x.Code == "RESET_PASSWORD" && x.Language == culture && x.IsActive)
                    .FirstOrDefaultAsync();

                if (template is null)
                {
                    Log.Warning("ForgotPassword - email template not found for culture {Culture}", culture);
                    return new ObjectResult(GenericResponse<object>.ErrorResponse(404, ErrorMessages.GetMessage("FP001"), "FP001", ErrorMessages.GetMessage("FP001"))) { StatusCode = 404 };
                }

                var subject = template.Subject;
                var body = template.Body
                    .Replace("{{FullName}}", user.UserName ?? user.Email)
                    .Replace("{{Email}}", user.Email)
                    .Replace("{{ResetLink}}", resetLink);

                // Create an audit log entry
                var auditEntry = new AuditLogDto
                {
                    TableName = "Users",
                    ActionType = "ForgotPassword",
                    KeyValues = $"UserId: {user.Id}",
                    NewValues = $"Email: {user.Email}",
                    ActionTimestamp = DateTime.UtcNow,
                    UserId = user.Id,
                    UserName = user.UserName,
                    IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown"
                };

                // Log audit entry asynchronously
                await auditLogService.LogAsync(auditEntry);

                Log.Information("ForgotPassword email requested for: {Email}", user.Email);

                // Send email asynchronously
                BackgroundJob.Enqueue(() => emailService.SendAsync(user.Email!, subject, body));

                return new ObjectResult(GenericResponse<object>.SuccessResponse(200, string.Empty, null!)) { StatusCode = 200 };
            }
            catch (Exception ex)
            {
                Log.Error(ex, "ForgotPasswordController Index failed for email: {Email}", model.Email);
            }

            return new ObjectResult(GenericResponse<object>.ErrorResponse(404, ErrorMessages.GetMessage("FP001"), "FP001", ErrorMessages.GetMessage("FP001"))) { StatusCode = 404 };
        }
    }
}
