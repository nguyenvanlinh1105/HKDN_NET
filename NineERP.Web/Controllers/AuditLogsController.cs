using MediatR;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NineERP.Application.Dtos.AuditLog;
using NineERP.Application.Features.AuditLogsFeature.Queries;
using NineERP.Application.Request;
using NineERP.Domain.Enums;

namespace NineERP.Web.Controllers;

[Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
public class AuditLogsController(IMediator mediator) : BaseController
{
    [Authorize(Policy = $"Permission:{PermissionValue.AuditLogs.View}")]
    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    [Authorize(Policy = $"Permission:{PermissionValue.AuditLogs.View}")]
    public async Task<IActionResult> GetAuditLogsPaging([FromQuery] AuditLogRequest request)
    {
        var result = await mediator.Send(new GetAuditLogsPaginationQuery(request));
        return Json(result);
    }

    [HttpGet]
    [Authorize(Policy = $"Permission:{PermissionValue.AuditLogs.View}")]
    public async Task<IActionResult> ExportExcel([FromQuery] AuditLogRequest request)
    {
        var result = await mediator.Send(new ExportAuditLogsQuery(request));
        return File(result, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "audit_logs.xlsx");
    }
}