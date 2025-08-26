using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NineERP.Application.Features.GeneralSettingsFeature.Commands;
using NineERP.Application.Features.GeneralSettingsFeature.Queries;
using NineERP.Application.Dtos.GeneralSetting;
using Microsoft.AspNetCore.Authentication.Cookies;
using NineERP.Domain.Enums;

namespace NineERP.Web.Controllers;

[Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
public class GeneralSettingsController(IMediator mediator) : Controller
{
    [Authorize(Policy = $"Permission:{PermissionValue.GeneralSettings.View}")]
    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    [Route("GeneralSettings/GetData")]
    [Authorize(Policy = $"Permission:{PermissionValue.GeneralSettings.View}")]
    public async Task<IActionResult> GetData()
    {
        var result = await mediator.Send(new GetGeneralSettingsQuery());
        return Ok(result.Data); // Trả về dữ liệu thuần để JS bind vào form
    }

    [HttpPost]
    [Authorize(Policy = $"Permission:{PermissionValue.GeneralSettings.Update}")]
    public async Task<IActionResult> SaveGeneralSettings([FromForm] GeneralSettingRequest request)
    {
        var result = await mediator.Send(new UpdateGeneralSettingsCommand(request));
        return Json(result);
    }
}
