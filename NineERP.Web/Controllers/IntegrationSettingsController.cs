using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using NineERP.Application.Dtos.IntegrationSetting;
using NineERP.Application.Features.IntegrationSettingsFeature.Commands;
using NineERP.Domain.Enums;
using NineERP.Application.Features.IntegrationSettingsFeature.Queries;

namespace NineERP.Web.Controllers;

[Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
[Route("IntegrationSettings")]
public class IntegrationSettingsController(IMediator mediator) : Controller
{
    [HttpGet("")]
    [Authorize(Policy = $"Permission:{PermissionValue.IntegrationSettings.View}")]
    public IActionResult Index()
    {
        return View();
    }

    [HttpGet("GetAll")]
    [Authorize(Policy = $"Permission:{PermissionValue.IntegrationSettings.View}")]
    public async Task<IActionResult> GetAll()
    {
        var result = await mediator.Send(new GetAllIntegrationSettingsQuery());
        return Ok(result.Data); // Dữ liệu: List<IntegrationSettingDto>
    }

    [HttpPost("Save")]
    [Authorize(Policy = $"Permission:{PermissionValue.IntegrationSettings.Update}")]
    public async Task<IActionResult> Save([FromForm] IntegrationSettingRequest request)
    {
        var result = await mediator.Send(new UpdateIntegrationSettingCommand(request));
        return Json(result);
    }

    [HttpPost("TestUpload")]
    [Authorize(Policy = $"Permission:{PermissionValue.IntegrationSettings.TestUpload}")]
    public async Task<IActionResult> TestUpload([FromForm] IFormFile file, [FromForm] string? subFolder)
    {
        var result = await mediator.Send(new TestUploadIntegrationCommand(file, subFolder));
        return Json(result);
    }

    [HttpPost("TestDrive")]
    [Authorize(Policy = $"Permission:{PermissionValue.IntegrationSettings.TestUpload}")]
    public async Task<IActionResult> TestGoogleDrive()
    {
        var result = await mediator.Send(new TestIntegrationSettingCommand());
        return Json(result);
    }

    [HttpPost("TestS3")]
    [Authorize(Policy = $"Permission:{PermissionValue.IntegrationSettings.TestUpload}")]
    public async Task<IActionResult> TestS3Bucket()
    {
        var result = await mediator.Send(new CheckS3BucketAccessCommand());
        return Json(result);
    }

    [HttpPost("ValidateRecaptcha")]
    [Authorize(Policy = $"Permission:{PermissionValue.IntegrationSettings.TestUpload}")]
    public async Task<IActionResult> ValidateRecaptchaKey()
    {
        var result = await mediator.Send(new ValidateReCaptchaKeyCommand());
        return Json(result);
    }
}
